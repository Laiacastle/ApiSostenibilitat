using Microsoft.AspNetCore.Identity;

namespace ApiSostenibilitatDef.Tools
{
    public class RoleTools
    {
        public static async Task CrearRolsInicials(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] rols = { "Admin", "User", "Doctor" };
            foreach (var rol in rols)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new IdentityRole(rol));
                }
            }
        }
    }
}
