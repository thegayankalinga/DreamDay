using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models;

public class MessageThread
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    [MaxLength(200)]
    public required string Subject { get; set; }
        
    public DateTime CreatedAt { get; set; } = DateTime.Now;
        
    [Required]
    [MaxLength(450)]
    public required string CreatorId { get; set; }
        
    [ForeignKey("CreatorId")]
    public AppUser? Creator { get; set; }
        
    [Required]
    [MaxLength(450)]
    public required string RecipientId { get; set; }
        
    [ForeignKey("RecipientId")]
    public AppUser? Recipient { get; set; }
        
    // Navigation property for messages in this thread
    public ICollection<Message> Messages { get; set; } = new List<Message>();
        
    // Flag to indicate if thread is archived
    public bool IsArchivedByCreator { get; set; } = false;
    public bool IsArchivedByRecipient { get; set; } = false;
}