using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    [AllowAnonymous]
    public class IndexModel : DI_BasePageModel
    {

        public IndexModel(ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
            
        }

        public IList<Invoice> Invoice { get;set; } = default!;

        public async Task OnGetAsync()
        {
            //Return all invoices
            var invoices = from i in Context.Invoice
                           select i;

            //If the current user is a Manager
            var isManager = User.IsInRole(Constants.InvoiceManagerRole);

            var currenUserId = UserManager.GetUserId(User);

            //If you are not the Manager
            if (isManager == false) 
            {
                //Only return what has been created by me
                invoices = invoices.Where(i => i.CreatorId == currenUserId);
            }


            Invoice = await invoices.ToListAsync();
        } 
    }
}
