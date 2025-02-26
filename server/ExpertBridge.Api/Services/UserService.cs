using ExpertBridge.Core;
using ExpertBridge.Core.DTOs.Requests;
using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;
using FluentValidation;

namespace ExpertBridge.Api.Services;

public class UserService(
    IEntityRepository<User> userRepository,
    IValidator<RegisterUserRequest> registerUserRequestValidator
    ) : IUserService
{
    public async Task<UserResponse> GetUserByIdentityProviderId(string identityProviderId)
    {
        var user = await userRepository.GetFirstAsNoTrackingAsync(user => user.ProviderId == identityProviderId)
            ?? throw new UserNotFoundException("User not found");
        return new UserResponse(user);
    }

    public async Task<UserResponse> RegisterNewUser(RegisterUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        var validationResult = await registerUserRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            ProviderId = request.FirebaseId,
            Email = request.Email,
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            IsBanned = false,
            IsDeleted = false
        };
        await userRepository.AddAsync(user);
        return new UserResponse(user);
    }

    public Task<UserResponse> UpdateUserAsync(UpdateUserRequest request)
    {
        throw new NotImplementedException();
    }
}
