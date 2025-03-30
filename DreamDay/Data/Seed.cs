using DreamDay.Models;
using Microsoft.AspNetCore.Identity;

namespace DreamDay.Data;

public class Seed
{
    public static void SeedData(IApplicationBuilder applicationBuilder){}

    public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.Couple))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Couple));
                if(!await roleManager.RoleExistsAsync(UserRoles.Planner))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Planner));

                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                string adminUserEmail = "gayan@dreamday.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new AppUser()
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

                string appCoupleEmail = "couple@dreamday.com";

                var appUser = await userManager.FindByEmailAsync(appCoupleEmail);
                if (appUser == null)
                {
                    var newAppUser = new AppUser()
                    {
                        UserName = appCoupleEmail,
                        Email = appCoupleEmail,
                        EmailConfirmed = true,
                        FirstName = "Couple",
                        LastName = "One"
                    };
                    await userManager.CreateAsync(newAppUser, "Test@123");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.Couple);
                }
                
                string appPlannerEmail = "planner@dreamday.com";

                var appPlanner = await userManager.FindByEmailAsync(appPlannerEmail);
                if (appPlanner == null)
                {
                    var newAppUser = new AppUser()
                    {
                        UserName = appPlannerEmail,
                        Email = appPlannerEmail,
                        EmailConfirmed = true,
                        FirstName = "Planner",
                        LastName = "One"
                    };
                    await userManager.CreateAsync(newAppUser, "Test@123");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.Planner);
                }
        }
    }
}