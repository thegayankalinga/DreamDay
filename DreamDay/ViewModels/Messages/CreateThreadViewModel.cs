using System.ComponentModel.DataAnnotations;

namespace DreamDay.ViewModels.Messages;

public class CreateThreadViewModel
{
    [Required(ErrorMessage = "Subject is required")]
    [StringLength(200, ErrorMessage = "Subject cannot be longer than 200 characters")]
    public string Subject { get; set; }
        
    [Required(ErrorMessage = "Message content is required")]
    public string MessageContent { get; set; }
        
    [Required(ErrorMessage = "Recipient is required")]
    public string RecipientId { get; set; }
        
    public List<UserSelectListItem> AvailableRecipients { get; set; } = new List<UserSelectListItem>();
}