using Aggregation.Backend.Domain.Entities;
using Aggregation.Backend.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aggregation.Backend.WebApi.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly TokenGenerator _tokenGenerator;
        private readonly UserManager<AggregationBackendUser> _userManager;

        public IndexModel(TokenGenerator tokenGenerator, UserManager<AggregationBackendUser> userManager)
        {
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
        }

        public string AccessToken { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            AccessToken = _tokenGenerator.GenerateJwtToken(user.Email ?? user.UserName).AccessToken;
            return Page();
        }
    }
}
