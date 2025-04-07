using System.ComponentModel.DataAnnotations;

namespace DreamDay.ViewModels.Messages;

public class ThreadDetailViewModel
{
    public int ThreadId { get; set; }
    public string Subject { get; set; }
    public string OtherPersonName { get; set; }
    public string OtherPersonRole { get; set; }
    public string OtherPersonId { get; set; }
    public List<MessageViewModel> Messages { get; set; } = new List<MessageViewModel>();
        
    [Required(ErrorMessage = "Message content is required")]
    public string NewMessageContent { get; set; }
}