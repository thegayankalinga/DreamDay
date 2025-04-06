using DreamDay.Data.Enums;
using DreamDay.Interfaces;
using DreamDay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using DreamDay.Data;

namespace DreamDay.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class VendorController : Controller
{
    private readonly IVendorRepository _vendorRepository;

    public VendorController(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    // GET: Vendor
    public async Task<IActionResult> Index()
    {
        var vendors = await _vendorRepository.GetAllVendorsAsync();
        return View(vendors);
    }

    // GET: Vendor/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id);
        if (vendor == null)
        {
            return NotFound();
        }
        return View(vendor);
    }

    // GET: Vendor/Create
    public IActionResult Create()
    {
        ViewBag.ServiceTypes = Enum.GetValues(typeof(VendorServiceTypes));
        return View();
    }

    // POST: Vendor/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vendor vendor)
    {
        if (ModelState.IsValid)
        {
            await _vendorRepository.AddAsync(vendor);
            return RedirectToAction(nameof(Index));
        }
        ViewBag.ServiceTypes = Enum.GetValues(typeof(VendorServiceTypes));
        return View(vendor);
    }

    // GET: Vendor/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id);
        if (vendor == null)
        {
            return NotFound();
        }
        ViewBag.ServiceTypes = Enum.GetValues(typeof(VendorServiceTypes));
        return View(vendor);
    }

    // POST: Vendor/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vendor vendor)
    {
        if (id != vendor.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _vendorRepository.UpdateAsync(vendor, id);
            return RedirectToAction(nameof(Index));
        }
        ViewBag.ServiceTypes = Enum.GetValues(typeof(VendorServiceTypes));
        return View(vendor);
    }

    // GET: Vendor/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id);
        if (vendor == null)
        {
            return NotFound();
        }
        return View(vendor);
    }

    // POST: Vendor/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _vendorRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // GET: Vendor/FilterByType
    public async Task<IActionResult> FilterByType(VendorServiceTypes serviceType)
    {
        var vendors = await _vendorRepository.GetVendorsByServiceTypeAsync(serviceType);
        ViewBag.CurrentFilter = serviceType;
        ViewBag.ServiceTypes = Enum.GetValues(typeof(VendorServiceTypes));
        return View("Index", vendors);
    }
}