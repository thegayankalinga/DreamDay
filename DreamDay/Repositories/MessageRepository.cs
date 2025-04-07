using DreamDay.Data;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thread operations
        public async Task<IEnumerable<MessageThread>> GetUserThreadsAsync(string userId)
        {
            return await _context.MessageThreads
                .Include(t => t.Creator)
                .Include(t => t.Recipient)
                .Include(t => t.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .Where(t => (t.CreatorId == userId && !t.IsArchivedByCreator) || 
                           (t.RecipientId == userId && !t.IsArchivedByRecipient))
                .OrderByDescending(t => t.Messages.Max(m => m.CreatedAt))
                .ToListAsync();
        }

        public async Task<MessageThread?> GetThreadByIdAsync(int threadId)
        {
            return await _context.MessageThreads
                .Include(t => t.Creator)
                .Include(t => t.Recipient)
                .Include(t => t.Messages)
                .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(t => t.Id == threadId);
        }

        public async Task<MessageThread> CreateThreadAsync(MessageThread thread)
        {
            _context.MessageThreads.Add(thread);
            await _context.SaveChangesAsync();
            return thread;
        }

        public async Task<bool> ArchiveThreadAsync(int threadId, string userId, bool archive)
        {
            var thread = await _context.MessageThreads.FindAsync(threadId);
            if (thread == null)
                return false;

            if (thread.CreatorId == userId)
                thread.IsArchivedByCreator = archive;
            else if (thread.RecipientId == userId)
                thread.IsArchivedByRecipient = archive;
            else
                return false;

            await _context.SaveChangesAsync();
            return true;
        }

        // Message operations
        public async Task<IEnumerable<Message>> GetThreadMessagesAsync(int threadId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.MessageThreadId == threadId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<Message?> GetMessageByIdAsync(int messageId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Thread)
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> MarkMessageAsReadAsync(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
                return false;

            message.IsReadByRecipient = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // Count unread messages
        public async Task<int> GetUnreadMessageCountAsync(string userId)
        {
            return await _context.Messages
                .Include(m => m.Thread)
                .Where(m => m.Thread.RecipientId == userId && !m.IsReadByRecipient)
                .CountAsync();
        }

        // Get or create thread between two specific users
        public async Task<MessageThread> GetOrCreateThreadAsync(string userId1, string userId2, string subject)
        {
            // Look for existing threads between the two users
            var existingThread = await _context.MessageThreads
                .Where(t => 
                    (t.CreatorId == userId1 && t.RecipientId == userId2) ||
                    (t.CreatorId == userId2 && t.RecipientId == userId1))
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (existingThread != null)
                return existingThread;

            // Create new thread if none exists
            var newThread = new MessageThread
            {
                Subject = subject,
                CreatorId = userId1,
                RecipientId = userId2,
                CreatedAt = DateTime.Now
            };

            _context.MessageThreads.Add(newThread);
            await _context.SaveChangesAsync();
            return newThread;
        }
    }
}