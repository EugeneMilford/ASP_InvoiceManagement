using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Authorization;

namespace IdentityApp.Data
{
    public class SeedData
    {
        public static async Task Initialize(
            IServiceProvider serviceProvider,
            string password = "Test@1234")
        {
            using(var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>())) 
            {
                var managerUid = await EnsureUser(serviceProvider, "manager@demo.com", password);
                await EnsureRole(serviceProvider, managerUid, Constants.InvoiceManagerRole);
            }
        }


        //Creates a new account
        private static async Task<string> EnsureUser(
            IServiceProvider serviceProvider,
            string userName, string initPw)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(userName);

            //If we do not have a manager account
            if (user == null) 
            {
                //Create new user
                user = new IdentityUser
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, initPw);
            }

            //If the user is not created
            if (user == null) 
                throw new Exception("User was not created...");

            return user.Id;
        }

        //Creates a new role for the user
        private static async Task<IdentityResult> EnsureRole(
            IServiceProvider serviceProvider, 
            string uid, 
            string role) 
        {
            //Get the roleManager to check if we have the role
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            IdentityResult ir;

            //If the specific role does not exist
            if (await roleManager.RoleExistsAsync(role) == false) 
            {
                //create the role
                ir = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(uid);

            if (user == null) 
                throw new Exception("User does not exist..."); 
            
            //Add the user to the role
            ir = await userManager.AddToRoleAsync(user, role);

            return ir;

        }
    }
}
