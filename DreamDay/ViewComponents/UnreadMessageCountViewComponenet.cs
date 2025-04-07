using DreamDay.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DreamDay.ViewComponents
{
    public class UnreadMessageCountViewComponent : ViewComponent
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserProfileRepository _userProfileRepository;

        public UnreadMessageCountViewComponent(
            IMessageRepository messageRepository,
            IUserProfileRepository userProfileRepository)
        {
            _messageRepository = messageRepository;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = _userProfileRepository.CurrentUser;
            if (user == null)
                return View(0);

            var unreadCount = await _messageRepository.GetUnreadMessageCountAsync(user.Id);
            return View(unreadCount);
        }
    }
}