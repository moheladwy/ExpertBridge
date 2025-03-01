using ExpertBridge.Core;
using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.DTOs.Requests.UpdateUserRequest;
using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;
using FluentValidation;

namespace ExpertBridge.Api.Services;

public class UserService(
    IEntityRepository<User> userRepository,
    IProfileService profileService,
    IValidator<RegisterUserRequest> registerUserRequestValidator,
    IValidator<UpdateUserRequest> updateUserRequestValidator
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
        var validationResult = await registerUserRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            ProviderId = request.ProviderId,
            Email = request.Email,
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            IsBanned = false,
            IsDeleted = false
        };
        await userRepository.AddAsync(user);
        await profileService.CreateProfileAsync(user);
        return new UserResponse(user);
    }

    public async Task<UserResponse> UpdateUserAsync(UpdateUserRequest request)
    {
        var validationResult = await updateUserRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        var user = await userRepository.GetFirstAsync(u => u.ProviderId == request.ProviderId);
        if (user is null)
        {
            return await RegisterNewUser(new RegisterUserRequest
            {
                ProviderId = request.ProviderId,
                Email = request.Email,
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName
            });
        }
        // Update user properties if they are different from the request
        user.Email = request.Email ==  user.Email ? user.Email : request.Email;
        user.Username = request.Username == user.Username ? user.Username : request.Username;
        user.FirstName = request.FirstName == user.FirstName ? user.FirstName : request.FirstName;
        user.LastName = request.LastName == user.LastName ? user.LastName : request.LastName;
        user.PhoneNumber = request.PhoneNumber == user.PhoneNumber ? user.PhoneNumber : request.PhoneNumber;
        await userRepository.UpdateAsync(user);

        return new UserResponse(user);
    }

    public async Task DeleteUserAsync(string identityProviderId)
    {
        var user = await userRepository.GetFirstAsync(u => u.ProviderId == identityProviderId)
            ?? throw new UserNotFoundException("User not found");
        user.IsDeleted = true;
        await userRepository.UpdateAsync(user);
    }

    public async Task<bool> IsUserBannedAsync(string identityProviderId)
    {
        var user = await userRepository.GetFirstAsNoTrackingAsync(u => u.ProviderId == identityProviderId)
            ?? throw new UserNotFoundException("User not found");
        return user.IsBanned;
    }

    public async Task<bool> IsUserVerifiedAsync(string email)
    {
        var user = await userRepository.GetFirstAsNoTrackingAsync(u => u.Email == email)
            ?? throw new UserNotFoundException("User not found");
        return user.IsEmailVerified;
    }

    public async Task<bool> IsUserDeletedAsync(string identityProviderId)
    {
        var user = await userRepository.GetFirstAsNoTrackingAsync(u => u.ProviderId == identityProviderId)
            ?? throw new UserNotFoundException("User not found");
        return user.IsDeleted;
    }

    public async Task BanUserAsync(string identityProviderId)
    {
        var user = await userRepository.GetFirstAsync(u => u.ProviderId == identityProviderId)
            ?? throw new UserNotFoundException("User not found");
        user.IsBanned = true;
        await userRepository.UpdateAsync(user);
    }

    public async Task VerifyUserAsync(string email)
    {
        var user = await userRepository.GetFirstAsync(u => u.Email == email)
            ?? throw new UserNotFoundException("User not found");
        user.IsEmailVerified = true;
        await userRepository.UpdateAsync(user);
    }
}
