using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IVenueRepository
{
    Task<List<Venue>> GetAllVenuesAsync();
    Task<Venue?> GetByIdAsync(int id);
    Task<bool> AddAsync(Venue venue);
    Task<bool> UpdateAsync(Venue venue, int venueId);
    Task<bool> DeleteAsync(int venueId);
}