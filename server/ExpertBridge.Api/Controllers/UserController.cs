using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UserController(
    IUserService userService,
    IValidator<RegisterUserRequest> registerUserRequestValidator
    ) : ControllerBase
{
    [HttpGet("get/{firebaseId}")]
    public async Task<User> GetUserByFirebaseId([FromRoute] string firebaseId)
    {
        ArgumentException.ThrowIfNullOrEmpty(firebaseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(firebaseId);

        return await userService.GetUserByFirebaseId(firebaseId);
    }

    [HttpPost("register")]
    public async Task<User> RegisterNewUser([FromBody] RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationResult = await registerUserRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        return await userService.RegisterNewUser(request);
    }

}
