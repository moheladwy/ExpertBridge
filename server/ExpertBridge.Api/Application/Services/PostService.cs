using ExpertBridge.Api.Core;
using ExpertBridge.Api.Core.DTOs.Requests.CreatePost;
using ExpertBridge.Api.Core.DTOs.Responses;
using ExpertBridge.Api.Core.Entities.Post;
using ExpertBridge.Api.Core.Interfaces.Repositories;
using ExpertBridge.Api.Core.Interfaces.Services;
using FluentValidation;

namespace ExpertBridge.Api.Application.Services;

public class PostService(
    IEntityRepository<Post> postRepository,
    IValidator<CreatePostRequest> createPostRequestValidator)
    : IPostService
{
    public async Task<PostResponse> GetByIdAsync(string id)
    {
        var post = await postRepository.GetByIdAsNoTrackingAsync(id);
        if (post is null) throw new PostNotFoundException("Post not found");
        return new PostResponse(post);
    }
}
