using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity;

namespace AcxiomCRM.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            string[] roles = { "Admin", "Manager", "Sales Executive" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = Environment.GetEnvironmentVariable("ACX_ADMIN_EMAIL");

            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                var user = await userManager.FindByEmailAsync(adminEmail);

                if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}