using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IWeddingChecklistRepository
{
    Task<IEnumerable<WeddingChecklist>> GetChecklistsByUserIdAsync(string userId);
    Task<WeddingChecklist?> GetChecklistByIdAsync(int id);
    Task CreateChecklistAsync(WeddingChecklist checklist);
    Task UpdateChecklistAsync(WeddingChecklist checklist);
    Task DeleteChecklistAsync(int id);

    Task<WeddingChecklistItem?> GetItemByIdAsync(int itemId);
    Task AddItemAsync(WeddingChecklistItem item);
    Task UpdateItemAsync(WeddingChecklistItem item);
    Task DeleteItemAsync(int itemId);
}