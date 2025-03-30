using DreamDay.Data;
using DreamDay.Models;
using Microsoft.AspNetCore.Identity;

public class Seed
{
    public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Roles
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.Couple))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Couple));
            if (!await roleManager.RoleExistsAsync(UserRoles.Planner))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Planner));

            // Users
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            // Admin
            string adminUserEmail = "gayan@dreamday.com";
            var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
            if (adminUser == null)
            {
                var newAdminUser = new AppUser
                {
                    UserName = adminUserEmail,
                    Email = adminUserEmail,
                    EmailConfirmed = true,
                    FirstName = "Gayan",
                    LastName = "Admin",
                };
                await userManager.CreateAsync(newAdminUser, "Test@123");
                await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
            }

            // Couple
            string coupleEmail = "couple@dreamday.com";
            var coupleUser = await userManager.FindByEmailAsync(coupleEmail);
            if (coupleUser == null)
            {
                coupleUser = new AppUser
                {
                    UserName = coupleEmail,
                    Email = coupleEmail,
                    EmailConfirmed = true,
                    FirstName = "Couple",
                    LastName = "One"
                };
                await userManager.CreateAsync(coupleUser, "Test@123");
                await userManager.AddToRoleAsync(coupleUser, UserRoles.Couple);
            }

            // Couple Profile
            if (!context.CoupleProfiles.Any())
            {
                context.CoupleProfiles.Add(new CoupleProfile
                {
                    AppUserId = coupleUser.Id,
                    WeddingDate = new DateOnly(2025, 10, 10),
                    PartnerName = "Jamie Doe"
                });
            }

            // Planners
            string[] plannerEmails =
            {
                "planner1@dreamday.com",
                "planner2@dreamday.com",
                "planner3@dreamday.com",
                "planner4@dreamday.com"
            };

            for (int i = 0; i < plannerEmails.Length; i++)
            {
                var plannerUser = await userManager.FindByEmailAsync(plannerEmails[i]);
                if (plannerUser == null)
                {
                    plannerUser = new AppUser
                    {
                        UserName = plannerEmails[i],
                        Email = plannerEmails[i],
                        EmailConfirmed = true,
                        FirstName = $"Planner",
                        LastName = $"#{i + 1}"
                    };
                    await userManager.CreateAsync(plannerUser, "Test@123");
                    await userManager.AddToRoleAsync(plannerUser, UserRoles.Planner);
                }

                // Add planner profile if not exists
                if (!context.PlannerProfiles.Any(p => p.AppUserId == plannerUser.Id))
                {
                    context.PlannerProfiles.Add(new PlannerProfile
                    {
                        AppUserId = plannerUser.Id,
                        IsApproved = (i == 0) // only first is approved
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
