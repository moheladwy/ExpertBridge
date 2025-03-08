using ExpertBridge.Core;
using ExpertBridge.Core.DTOs.Requests.CreatePost;
using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Entities.Post;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;
using FluentValidation;

namespace ExpertBridge.Application.Services;

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
