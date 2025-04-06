using DreamDay.Data;
using DreamDay.Data.Enums;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamDay.Repositories;

public class VendorRepository: IVendorRepository
{
    private readonly ApplicationDbContext _context;

    public VendorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vendor>> GetAllVendorsAsync()
    {
        return await _context.Vendors.ToListAsync();
    }

    public async Task<Vendor?> GetByIdAsync(int id)
    {
        return await _context.Vendors
            .Include(v => v.WeddingEventVendors)
            .ThenInclude(wev => wev.WeddingEvent)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<bool> AddAsync(Vendor vendor)
    {
        _context.Vendors.Add(vendor);
        return await _context.SaveChangesAsync() == 1;
    }

    public async Task<bool> UpdateAsync(Vendor vendor, int vendorId)
    {
        var existingVendor = await _context.Vendors.FindAsync(vendorId);
        if (existingVendor == null) return false;

        existingVendor.Name = vendor.Name;
        existingVendor.ContactInfo = vendor.ContactInfo;
        existingVendor.ServiceType = vendor.ServiceType;

        return await _context.SaveChangesAsync() == 1;
    }

    public async Task<bool> DeleteAsync(int vendorId)
    {
        var vendor = await _context.Vendors.FindAsync(vendorId);
        if (vendor == null) return false;

        _context.Vendors.Remove(vendor);
        return await _context.SaveChangesAsync() == 1;
    }

    public async Task<List<Vendor>> GetVendorsByServiceTypeAsync(VendorServiceTypes serviceType)
    {
        return await _context.Vendors
            .Where(v => v.ServiceType == serviceType)
            .ToListAsync();
    }
}