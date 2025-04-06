using DreamDay.Data.Enums;
using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IVendorRepository
{
    Task<List<Vendor>> GetAllVendorsAsync();
    Task<Vendor?> GetByIdAsync(int id);
    Task<bool> AddAsync(Vendor vendor);
    Task<bool> UpdateAsync(Vendor vendor, int vendorId);
    Task<bool> DeleteAsync(int vendorId);
    Task<List<Vendor>> GetVendorsByServiceTypeAsync(VendorServiceTypes serviceType);
} 
