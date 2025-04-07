namespace DreamDay.ViewModels.Messages;

public class MessageListViewModel
{
    public List<MessageThredViewModel> Threads { get; set; } = new List<MessageThredViewModel>();
    public int UnreadCount { get; set; }
}