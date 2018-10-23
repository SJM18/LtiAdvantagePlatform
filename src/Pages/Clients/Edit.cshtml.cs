﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AdvantagePlatform.Data;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AdvantagePlatform.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _appContext;
        private readonly IConfigurationDbContext _identityContext;
        private readonly UserManager<AdvantagePlatformUser> _userManager;

        public EditModel(ApplicationDbContext appContext, IConfigurationDbContext identityContext, UserManager<AdvantagePlatformUser> userManager)
        {
            _appContext = appContext;
            _identityContext = identityContext;
            _userManager = userManager;
        }

        [BindProperty]
        public ClientModel Client { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var clientSecret = await _appContext.ClientSecretText
                //.Where(secret => user.ClientIds.Contains(secret.ClientId))
                .SingleOrDefaultAsync(secret => secret.ClientId == id);

            if (clientSecret == null)
            {
                return NotFound();
            }
            
            var client = await _identityContext.Clients
                .Include(c => c.Properties)
                .Where(c => c.Properties.Any(p => p.Value == user.Id))
                .SingleOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            Client = new ClientModel
            {
                Id = client.Id,
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                ClientSecret = clientSecret.Secret 
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var clientEntity = await _identityContext.Clients.FindAsync(Client.Id);

            // Can only change the ClientName
            clientEntity.ClientName = Client.ClientName; 
            
            _identityContext.Clients.Attach(clientEntity).State = EntityState.Modified;
            await _identityContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
