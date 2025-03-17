using ExpertBridge.Api.Core.DTOs.Responses;

namespace ExpertBridge.Api.Core.Interfaces.Services;

public interface IPostService
{
    Task<PostResponse> GetByIdAsync(string id);
}
