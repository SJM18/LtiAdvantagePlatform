﻿using System.Threading.Tasks;
using AdvantagePlatform.Data;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace AdvantagePlatform.Pages.Tools
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _appContext;
        private readonly IConfigurationDbContext _identityContext;
        private readonly UserManager<AdvantagePlatformUser> _userManager;

        [BindProperty]
        public ToolModel Tool { get; set; }

        public CreateModel(
            ApplicationDbContext appContext, 
            IConfigurationDbContext identityContext, 
            UserManager<AdvantagePlatformUser> userManager)
        {
            _appContext = appContext;
            _identityContext = identityContext;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            Tool = new ToolModel
            {
                DeploymentId = GenerateRandomString(8)
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(Tool.ToolClientSecret))
            {
                ModelState.AddModelError("Tool.ToolClientSecret", "The Tool Client Secret field is required.");
                return Page();
            }

            var client = new Client
            {
                ClientId = Tool.ToolClientId,
                ClientName = Tool.ToolName,
                ClientSecrets = { new Secret(Tool.ToolClientSecret.Sha256()) },

                AllowOfflineAccess = true,
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = new [] { "api1" }
            };

            var entity = client.ToEntity();

            await _identityContext.Clients.AddAsync(entity);
            await _identityContext.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            var tool = new Tool
            {
                DeploymentId = Tool.DeploymentId,
                IdentSvrClientId = entity.Id,
                ToolIssuer = Tool.ToolIssuer,
                ToolJsonWebKeysUrl = Tool.ToolJsonWebKeysUrl,
                ToolName = Tool.ToolName,
                ToolUrl = Tool.ToolUrl,
                UserId = user.Id
            };

            await _appContext.Tools.AddAsync(tool);
            await _appContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private static string GenerateRandomString(int length = 24)
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var buffer = new byte[length];
                rng.GetBytes(buffer);
                return Base64UrlEncoder.Encode(buffer);
            }
        }
    }
}