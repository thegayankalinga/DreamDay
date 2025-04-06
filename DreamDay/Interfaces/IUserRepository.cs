﻿using DreamDay.Models;

namespace DreamDay.Interfaces;

public interface IUserRepository
{
    Task<List<AppUser>> GetAllAsync();
    Task<AppUser?> GetByIdAsync(string id);
    Task<bool> UpdateAsync(AppUser user, string userId);
    Task TogglePlannerActivationAsync(string userId);
    Task<bool?> GetPlannerActivationStatusAsync(string userId);

    Task<bool> DeleteAsync(string id);
}