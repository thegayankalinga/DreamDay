using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using DreamDay.ViewModels.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DreamDay.Controllers;

public class MessageController : Controller
{
    
        private readonly IMessageRepository _messageRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public MessageController(
            IMessageRepository messageRepository,
            IUserProfileRepository userProfileRepository,
            UserManager<AppUser> userManager,
            ApplicationDbContext context)
        {
            _messageRepository = messageRepository;
            _userProfileRepository = userProfileRepository;
            _userManager = userManager;
            _context = context;
        }

        // GET: /Messages
        public async Task<IActionResult> Index()
        {
            var currentUser = _userProfileRepository.CurrentUser;
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            var threads = await _messageRepository.GetUserThreadsAsync(currentUser.Id);
            var unreadCount = await _messageRepository.GetUnreadMessageCountAsync(currentUser.Id);
            
            var viewModel = new MessageListViewModel()
            {
                UnreadCount = unreadCount,
                Threads = threads.Select(t => new MessageThredViewModel()
                {
                    Id = t.Id,
                    Subject = t.Subject,
                    CreatedAt = t.CreatedAt,
                    OtherPersonId = t.CreatorId == currentUser.Id ? t.RecipientId : t.CreatorId,
                    OtherPersonName = GetUserDisplayName(t.CreatorId == currentUser.Id ? t.Recipient : t.Creator),
                    OtherPersonRole = GetUserRole(t.CreatorId == currentUser.Id ? t.Recipient : t.Creator),
                    LatestMessageContent = t.Messages.FirstOrDefault()?.Content ?? "No messages yet",
                    LatestMessageDate = t.Messages.FirstOrDefault()?.CreatedAt ?? t.CreatedAt,
                    HasUnreadMessages = t.Messages.Any(m => m.SenderId != currentUser.Id && !m.IsReadByRecipient)
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: /Messages/Thread/5
        public async Task<IActionResult> Thread(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Home");

            var thread = await _messageRepository.GetThreadByIdAsync(id);
            if (thread == null)
                return NotFound();

            // Security check: User must be part of the conversation
            if (thread.CreatorId != currentUser.Id && thread.RecipientId != currentUser.Id)
                return Forbid();

            // Get the other person in the conversation
            var otherPerson = thread.CreatorId == currentUser.Id ? thread.Recipient : thread.Creator;

            // Mark messages as read
            foreach (var message in thread.Messages.Where(m => m.SenderId != currentUser.Id && !m.IsReadByRecipient))
            {
                await _messageRepository.MarkMessageAsReadAsync(message.Id);
            }

            var viewModel = new ThreadDetailViewModel
            {
                ThreadId = thread.Id,
                Subject = thread.Subject,
                OtherPersonName = GetUserDisplayName(otherPerson),
                OtherPersonRole = GetUserRole(otherPerson),
                OtherPersonId = otherPerson.Id,
                Messages = thread.Messages.Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    SenderName = GetUserDisplayName(m.Sender),
                    SenderRole = GetUserRole(m.Sender),
                    IsFromCurrentUser = m.SenderId == currentUser.Id
                }).OrderBy(m => m.CreatedAt).ToList()
            };

            return View(viewModel);
        }

        public class MessageViewModels
        {
        }

        // POST: /Messages/Reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(ThreadDetailViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Home");

            var thread = await _messageRepository.GetThreadByIdAsync(model.ThreadId);
            if (thread == null)
                return NotFound();

            // Security check: User must be part of the conversation
            if (thread.CreatorId != currentUser.Id && thread.RecipientId != currentUser.Id)
                return Forbid();

            if (string.IsNullOrWhiteSpace(model.NewMessageContent))
            {
                ModelState.AddModelError("NewMessageContent", "Message content is required");
                return RedirectToAction(nameof(Thread), new { id = model.ThreadId });
            }

            var message = new Message
            {
                Content = model.NewMessageContent,
                SenderId = currentUser.Id,
                MessageThreadId = model.ThreadId,
                CreatedAt = DateTime.Now
            };

            await _messageRepository.CreateMessageAsync(message);
            return RedirectToAction(nameof(Thread), new { id = model.ThreadId });
        }

        // GET: /Messages/Create
        public async Task<IActionResult> Create(string recipientId = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Home");

            var viewModel = new CreateThreadViewModel();
            
            // Populate available recipients based on user role
            if (User.IsInRole(UserRoles.Couple))
            {
                // Couples can message planners
                var planners = await _userManager.GetUsersInRoleAsync(UserRoles.Planner);
                viewModel.AvailableRecipients = planners.Select(p => new UserSelectListItem
                {
                    Id = p.Id,
                    Name = GetUserDisplayName(p),
                    Role = "Planner"
                }).ToList();
            }
            else if (User.IsInRole(UserRoles.Planner))
            {
                // Planners can message couples
                var couples = await _userManager.GetUsersInRoleAsync(UserRoles.Couple);
                viewModel.AvailableRecipients = couples.Select(c => new UserSelectListItem
                {
                    Id = c.Id,
                    Name = GetUserDisplayName(c),
                    Role = "Couple"
                }).ToList();
            }

            // Preselect recipient if provided
            if (!string.IsNullOrEmpty(recipientId))
            {
                viewModel.RecipientId = recipientId;
            }

            return View(viewModel);
        }

        // POST: /Messages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateThreadViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                // Repopulate available recipients
                if (User.IsInRole(UserRoles.Couple))
                {
                    var planners = await _userManager.GetUsersInRoleAsync(UserRoles.Planner);
                    model.AvailableRecipients = planners.Select(p => new UserSelectListItem
                    {
                        Id = p.Id,
                        Name = GetUserDisplayName(p),
                        Role = "Planner"
                    }).ToList();
                }
                else if (User.IsInRole(UserRoles.Planner))
                {
                    var couples = await _userManager.GetUsersInRoleAsync(UserRoles.Couple);
                    model.AvailableRecipients = couples.Select(c => new UserSelectListItem
                    {
                        Id = c.Id,
                        Name = GetUserDisplayName(c),
                        Role = "Couple"
                    }).ToList();
                }
                return View(model);
            }

            // Create thread
            var thread = new MessageThread
            {
                Subject = model.Subject,
                CreatorId = currentUser.Id,
                RecipientId = model.RecipientId,
                CreatedAt = DateTime.Now
            };

            var createdThread = await _messageRepository.CreateThreadAsync(thread);

            // Create initial message
            var message = new Message
            {
                Content = model.MessageContent,
                SenderId = currentUser.Id,
                MessageThreadId = createdThread.Id,
                CreatedAt = DateTime.Now
            };

            await _messageRepository.CreateMessageAsync(message);
            return RedirectToAction(nameof(Thread), new { id = createdThread.Id });
        }

        // POST: /Messages/Archive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToAction("Index", "Home");

            var success = await _messageRepository.ArchiveThreadAsync(id, currentUser.Id, true);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // Helper methods
        private string GetUserDisplayName(AppUser user)
        {
            if (user == null)
                return "Unknown User";

            // For couples, show both names if available
            if (user.CoupleProfile != null)
            {
                return $"{user.FirstName} & {user.CoupleProfile.PartnerName}";
            }

            // For planners or anyone else, just show their name
            return $"{user.FirstName} {user.LastName}".Trim();
        }

        private string GetUserRole(AppUser user)
        {
            if (user == null)
                return "Unknown";

            // This is a simplified version - you might want to use the actual role from the system
            if (user.CoupleProfile != null)
                return "Couple";
            if (user.PlannerProfile != null)
                return "Planner";
            
            return "User";
        }
    
}