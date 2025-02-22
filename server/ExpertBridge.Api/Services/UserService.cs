using ExpertBridge.Core;
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
    public async Task<UserResponse> GetUserByIdentityProviderId(string firebaseId)
    {
        var user = await userRepository.GetFirstAsNoTrackingAsync(user => user.ProviderId == firebaseId)
            ?? throw new UserNotFoundException("User not found");
        return new UserResponse(user);
    }

    public async Task<UserResponse> RegisterNewUser(RegisterUserRequest requestDto)
    {
        ArgumentNullException.ThrowIfNull(requestDto, nameof(requestDto));
        var validationResult = await registerUserRequestValidator.ValidateAsync(requestDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            ProviderId = requestDto.FirebaseId,
            Email = requestDto.Email,
            Username = requestDto.Username,
            FirstName = requestDto.FirstName,
            LastName = requestDto.LastName,
            CreatedAt = DateTime.UtcNow,
            IsBanned = false,
            IsDeleted = false
        };
        await userRepository.AddAsync(user);
        return new UserResponse(user);
    }
}
