#nullable disable

using System.Security.Claims;
using System.Threading;
using Aggregation.Backend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aggregation.Backend.WebApi.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<AggregationBackendUser> _signInManager;
        private readonly UserManager<AggregationBackendUser> _userManager;
        private readonly IUserStore<AggregationBackendUser> _userStore;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<AggregationBackendUser> signInManager,
            UserManager<AggregationBackendUser> userManager,
            IUserStore<AggregationBackendUser> userStore,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGet() => RedirectToPage("./Login");

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            returnUrl = Url.Content("~/");
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            // First time — auto-register using GitHub email
            var email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        ?? info.Principal.FindFirstValue("urn:github:login");

            if (string.IsNullOrEmpty(email))
            {
                ErrorMessage = "Could not retrieve email from GitHub.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var user = new AggregationBackendUser();
            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            ((IUserEmailStore<AggregationBackendUser>)_userStore).SetEmailAsync(user, email, CancellationToken.None).GetAwaiter().GetResult();

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                ErrorMessage = string.Join(" ", createResult.Errors.Select(e => e.Description));
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            await _userManager.AddLoginAsync(user, info);
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, email));
            _logger.LogInformation("User created via {LoginProvider}.", info.LoginProvider);

            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
            return LocalRedirect(returnUrl);
        }
    }
}
