using ExpertBridge.Core.DTOs.Requests.RegisterUser;
using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Entities.User;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;
using FluentValidation;

namespace ExpertBridge.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IValidator<RegisterUserRequest> registerUserRequestValidator
    ) : IUserService
{
    public async Task<UserResponse> GetUserByFirebaseId(string firebaseId)
    {
        var user = await userRepository.GetUserByFirebaseId(firebaseId);
        return new UserResponse(user);
    }

    public async Task<UserResponse> RegisterNewUser(RegisterUserRequest request)
    {
        var validationResult = await registerUserRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = new User
        {
            FirebaseId = request.FirebaseId,
            Email = request.Email,
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = request.CreatedAt
        };
        await userRepository.AddUser(user);
        return new UserResponse(user);
    }
}
