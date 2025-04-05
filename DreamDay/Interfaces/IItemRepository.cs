using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IItemRepository
{
    Task<List<ChecklistItem>> GetAllChecklistItemsByChecklistIdAsync(int checklistId);
    Task<ChecklistItem?> GetChecklistItemByIdAsync(int checklistItemId);
    Task<bool> AddChecklistItemAsync(ChecklistItem item);
    Task<bool> UpdateChecklistItem(ChecklistItem item, int checklistItemId);
    Task<bool> DeleteChecklistItem(int checklistItemId);
    
    Task<bool> UpdateConfirmStatus(ChecklistItem item, int checklistItemId);

}