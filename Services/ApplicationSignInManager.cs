using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AcxiomCRM.Services
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser>
    {
        private readonly AuditService _auditService;

        public ApplicationSignInManager(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation,
            AuditService auditService)
            : base(
                userManager,
                contextAccessor,
                claimsFactory,
                optionsAccessor,
                logger,
                schemes,
                confirmation)
        {
            _auditService = auditService;
        }

        public override async Task<SignInResult> PasswordSignInAsync(
            string userName,
            string password,
            bool isPersistent,
            bool lockoutOnFailure)
        {
            var result = await base.PasswordSignInAsync(
                userName,
                password,
                isPersistent,
                lockoutOnFailure);

            if (result.Succeeded)
            {
                await _auditService.LogAsync(
                    "Login Success",
                    "Authentication",
                    userName,
                    null,
                    null);
            }
            else
            {
                await _auditService.LogAsync(
                    "Login Failed",
                    "Authentication",
                    userName,
                    null,
                    null);
            }

            return result;
        }

        public override async Task SignOutAsync()
        {
            var userName = Context.User?.Identity?.Name ?? "";

            await _auditService.LogAsync(
                "Logout",
                "Authentication",
                userName,
                null,
                null);

            await base.SignOutAsync();
        }
    }
}
