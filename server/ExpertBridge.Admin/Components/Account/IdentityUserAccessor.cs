using Microsoft.AspNetCore.Identity;

namespace ExpertBridge.Admin.Components.Account;

internal sealed class IdentityUserAccessor(
    UserManager<Database.Admin> userManager,
    IdentityRedirectManager redirectManager)
{
    public async Task<Database.Admin> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser",
                $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
