using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IGuestRepository
{
    Task<List<Guest>> GetAllGuestUsingAppUserIdAsync(string userId);
    Task<List<Guest>> GetAllGuestUsingCoupleId(int coupleId);
    Task<Guest?> GetByGuestIdAsync(int id);
    Task<bool> AddGuestAsync(Guest guest);
    Task<bool> UpdateGuestAsync(Guest guest, int guestId);
    Task<bool> DeleteGuestAsync(int id);
}