using System.ComponentModel.DataAnnotations;
using DreamDay.Data.Enums;

namespace DreamDay.Models;

public class Vendor
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(100)]
    public required string Name { get; set; }
    
    
    public VendorServiceTypes ServiceType { get; set; } // e.g., Photographer, Makeup, Catering
    
    [MaxLength(15)]
    public string? ContactInfo { get; set; }
    
    public ICollection<WeddingEventVendor> WeddingEventVendors { get; set; } = new List<WeddingEventVendor>();

}