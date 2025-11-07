using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace ExpertBridge.Admin.Components.Account.Pages;

public partial class Register : ComponentBase
{
    private IEnumerable<IdentityError>? identityErrors;

    [Inject] private UserManager<Database.Admin> UserManager { get; set; }

    [Inject] private IUserStore<Database.Admin> UserStore { get; set; }

    [Inject] private SignInManager<Database.Admin> SignInManager { get; set; }

    [Inject] private IEmailSender<Database.Admin> EmailSender { get; set; }

    [Inject] private ILogger<Register> Logger { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }

    [Inject] private IdentityRedirectManager RedirectManager { get; set; }

    [SupplyParameterFromForm] private InputModel Input { get; } = new();

    [SupplyParameterFromQuery] private string? ReturnUrl { get; set; }

    private string? Message
    {
        get
        {
            return identityErrors is null
                ? null
                : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";
        }
    }

    public async Task RegisterUser(EditContext editContext)
    {
        var user = CreateUser();

        await UserStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        var emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
        var result = await UserManager.CreateAsync(user, Input.Password);

        if (!result.Succeeded)
        {
            identityErrors = result.Errors;
            return;
        }

        Logger.LogInformation("User created a new account with password.");

        var userId = await UserManager.GetUserIdAsync(user);
        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });

        await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

        if (UserManager.Options.SignIn.RequireConfirmedAccount)
        {
            RedirectManager.RedirectTo(
                "Account/RegisterConfirmation",
                new Dictionary<string, object?> { ["email"] = Input.Email, ["returnUrl"] = ReturnUrl });
        }

        await SignInManager.SignInAsync(user, false);
        RedirectManager.RedirectTo(ReturnUrl);
    }

    private Database.Admin CreateUser()
    {
        try
        {
            return Activator.CreateInstance<Database.Admin>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(Database.Admin)}'. " +
                                                $"Ensure that '{nameof(Database.Admin)}' is not an abstract class and has a parameterless constructor.");
        }
    }

    private IUserEmailStore<Database.Admin> GetEmailStore()
    {
        if (!UserManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }

        return (IUserEmailStore<Database.Admin>)UserStore;
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
