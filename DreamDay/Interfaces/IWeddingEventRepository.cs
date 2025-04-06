using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IWeddingEventRepository
{
    Task<List<WeddingEvent>> GetAllByUserIdAsync(string userId);
    Task<WeddingEvent?> GetByIdAsync(int id);
    Task<bool> AddAsync(WeddingEvent weddingEvent, string userId);
    Task<bool> UpdateAsync(WeddingEvent weddingEvent, int eventId);
    Task<bool> DeleteAsync(int id);
    
    Task<List<Vendor>> GetVendorsByEventIdAsync(int eventId);
    Task<bool> AssignVendorsToEventAsync(int eventId, List<int> vendorIds);

    Task<List<WeddingEvent>> GetAllAsync();
}