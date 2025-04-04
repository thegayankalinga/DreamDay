using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IChecklistRepository
{
    Task<List<Checklist>> GetAllChecklistsByUserAsync(string userId);

    Task<List<Checklist>> GetAllChecklistsAndItemsByUserAsync(string userId);
    
    Task<Checklist?> GetChecklistByIdAsync(int id);
    Task CreateChecklistAsync(Checklist checklist);
    Task<bool> UpdateChecklistAsync(Checklist checklist);
    Task<bool> DeleteChecklistAsync(int id);
    
}