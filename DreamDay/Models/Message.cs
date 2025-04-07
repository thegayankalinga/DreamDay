using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models;

public class Message
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    [MaxLength(450)]
    public required string Content { get; set; }
        
    public DateTime CreatedAt { get; set; } = DateTime.Now;
        
    // To track if the message has been read
    public bool IsReadByRecipient { get; set; } = false;
        
    // Foreign keys for sender and thread
    [Required]
    [MaxLength(450)]
    public required string SenderId { get; set; }
        
    [ForeignKey("SenderId")]
    public AppUser? Sender { get; set; }
        
    [Required]
    public int MessageThreadId { get; set; }
        
    [ForeignKey("MessageThreadId")]
    public MessageThread? Thread { get; set; }

}