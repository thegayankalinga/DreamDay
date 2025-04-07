namespace DreamDay.ViewModels.Messages;

public class MessageViewModel
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SenderName { get; set; }
    public string SenderRole { get; set; }
    public bool IsFromCurrentUser { get; set; }
}