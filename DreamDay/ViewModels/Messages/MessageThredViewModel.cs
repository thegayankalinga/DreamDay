namespace DreamDay.ViewModels.Messages;

public class MessageThredViewModel
{
    public int Id { get; set; }
    public string Subject { get; set; }
    public DateTime CreatedAt { get; set; }
        
    // Information about the other person in the conversation
    public string OtherPersonId { get; set; }
    public string OtherPersonName { get; set; }
    public string OtherPersonRole { get; set; }
        
    // Latest message info
    public string? LatestMessageContent { get; set; }
    public DateTime LatestMessageDate { get; set; }
    public bool HasUnreadMessages { get; set; }
}