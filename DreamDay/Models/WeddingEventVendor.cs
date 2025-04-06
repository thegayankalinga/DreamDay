using System.ComponentModel.DataAnnotations.Schema;

namespace DreamDay.Models;

public class WeddingEventVendor
{
    [ForeignKey("WeddingEvent")]
    public int TimelineEventId { get; set; }
    public WeddingEvent? WeddingEvent { get; set; }

    [ForeignKey("Vendor")]
    public int VendorId { get; set; }
    public Vendor? Vendor { get; set; }
}