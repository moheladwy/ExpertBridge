using ExpertBridge.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     Provides authentication-related operations for the API, including user login and registration.
/// </summary>
/// <remarks>
///     This controller utilizes the <c>FirebaseAuthService</c> to handle user authentication actions.
///     The controller serves as an entry point for authentication requests in the API.
///     It maps incoming HTTP requests to service methods for login and registration functionality.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    /// <summary>
    ///     An instance of the <see cref="FirebaseAuthService" /> that provides authentication functionalities.
    /// </summary>
    /// <remarks>
    ///     The service is utilized by the <see cref="AuthController" /> to handle authentication-related tasks,
    ///     including user login and registration. It acts as the backend service layer that communicates with
    ///     Firebase for executing these operations.
    /// </remarks>
    private readonly FirebaseAuthService _authService;

    /// <summary>
    ///     Provides authentication-related operations for users such as login and registration,
    ///     leveraging the FirebaseAuthService for backend tasks.
    /// </summary>
    /// <remarks>
    ///     - Handles incoming API requests for user authentication.
    ///     - Functions as a bridge between the client and the FirebaseAuthService for actions
    ///     such as login and registration.
    ///     - Implements HTTP POST methods to handle user data securely.
    /// </remarks>
    public AuthController(FirebaseAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    ///     Handles the user login operation by validating credentials and retrieving an authentication token.
    /// </summary>
    /// <param name="request">
    ///     An object containing the login details such as user email and password.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous login operation. The task result contains
    ///     the authentication token if the login is successful.
    /// </returns>
    [HttpPost("login")]
    public async Task<string> Login([FromBody] LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await _authService.LoginAsync(request.Email, request.Password);
    }

    /// <summary>
    ///     Handles the user registration process by accepting user details,
    ///     delegating the registration logic to the FirebaseAuthService,
    ///     and returning a unique user identifier after successful registration.
    /// </summary>
    /// <param name="request">The registration details provided by the user, including email and password.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains a unique string identifier for the newly registered user.
    /// </returns>
    [HttpPost("register")]
    public async Task<string> Register([FromBody] RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await _authService.RegisterAsync(request.Email, request.Password);
    }
}
