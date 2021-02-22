using DataAccess.Context.Enum;
using Domain.DomainModel;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DataAccess.Context
{
    public class IdentityInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityInitializer(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task SeedAsync()
        {
            await CreateDefaultAccount("Accounting", "acc2021@gmail.com", "Accounting#2021", "Accounting", RoleName.Accountant.ToString());
            await CreateDefaultAccount("Admin", "admin2021@gmail.com", "Admin#2021", "Administrator", RoleName.Admin.ToString());

            await _roleManager.CreateAsync(new IdentityRole(RoleName.Manager.ToString()));
            await _roleManager.CreateAsync(new IdentityRole(RoleName.Employee.ToString()));
        }

        private async Task CreateDefaultAccount(string login, string email, string password, string name, string role)
        {

            if ((await _userManager.FindByNameAsync(login)) == null)
            {
                var user = new User() { UserName = login, Email = email, FirstName = name, LastName = "", Role = role };
                var saveuser = await _userManager.CreateAsync(user, password);

                if (saveuser.Succeeded)
                {
                    if ((await _roleManager.FindByNameAsync(role)) == null)
                    {
                        var saverole = await _roleManager.CreateAsync(new IdentityRole(role));

                        if (saverole.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }
                }

            }
        }
    }
}
