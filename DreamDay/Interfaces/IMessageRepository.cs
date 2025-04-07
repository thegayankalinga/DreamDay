using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IMessageRepository
{
    // Thread operations
    Task<IEnumerable<MessageThread>> GetUserThreadsAsync(string userId);
    Task<MessageThread?> GetThreadByIdAsync(int threadId);
    Task<MessageThread> CreateThreadAsync(MessageThread thread);
    Task<bool> ArchiveThreadAsync(int threadId, string userId, bool archive);
        
    // Message operations
    Task<IEnumerable<Message>> GetThreadMessagesAsync(int threadId);
    Task<Message?> GetMessageByIdAsync(int messageId);
    Task<Message> CreateMessageAsync(Message message);
    Task<bool> MarkMessageAsReadAsync(int messageId);
        
    // Count unread messages
    Task<int> GetUnreadMessageCountAsync(string userId);
        
    // Get thread between two specific users
    Task<MessageThread> GetOrCreateThreadAsync(string userId1, string userId2, string subject);

}