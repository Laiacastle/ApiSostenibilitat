using ApiSostenibilitat;
using ApiSostenibilitat.Controllers;
using ApiSostenibilitat.Data;
using ApiSostenibilitat.Models;
using Microsoft.AspNetCore.Identity;

namespace ApiSostenibilitatDef.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider, UserManager<User> userManager)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var admin = new User
            {
                UserName = "Admin",
                Email = "admin@gmail.com",
                Name = "Admin",
                Surname = "Admin",
                Weight = 68,
                Exercise = ExerciciEnum.Molt,
                HoursSleep = 8,
                Age = 23,
            };

            var existingUser = userManager.FindByEmailAsync(admin.Email).Result;
            if (existingUser == null)
            {
                userManager.AddToRoleAsync(admin, "Admin");
                userManager.CreateAsync(admin, "admin").Wait(); // Asignando una contraseña
            }
        }
    }
}
