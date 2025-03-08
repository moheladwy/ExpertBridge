using ExpertBridge.Core.DTOs.Requests.CreatePost;
using ExpertBridge.Core.DTOs.Requests.DeleteFileFromPost;
using ExpertBridge.Core.DTOs.Requests.EditPost;
using ExpertBridge.Core.DTOs.Requests.ReportPost;
using ExpertBridge.Core.DTOs.Responses;
using ExpertBridge.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostController(IPostService service) : ControllerBase
{
    [HttpGet("get/{postId}")]
    public async Task<PostResponse> Get([FromRoute] string postId)
    {
        ArgumentException.ThrowIfNullOrEmpty(postId, nameof(postId));
        return await service.GetByIdAsync(postId);
    }

    [HttpGet("get-all/{identityId}")]
    public async Task<IEnumerable<PostResponse>> GetAll([FromRoute] string identityId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("create")]
    public async Task<PostResponse> Create([FromBody] CreatePostRequest createPostRequest)
    {
        throw new NotImplementedException();
    }

    [HttpPost("up-vote/{postId}")]
    public async Task<IActionResult> UpVote()
    {
        throw new NotImplementedException();
    }

    [HttpPost("down-vote/{postId}")]
    public async Task<IActionResult> DownVote()
    {
        throw new NotImplementedException();
    }

    [HttpPost("attach/{postId}")]
    public async Task<AttachFileToPostResponse> AttachFile(IFormFile file, [FromRoute] string postId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("delete-file")]
    public async Task<IActionResult> DeleteFile([FromBody] DeleteFileFromPostRequest deleteFileFromPostRequest)
    {
        throw new NotImplementedException();
    }

    [HttpPut("edit")]
    public async Task<PostResponse> Edit([FromBody] EditPostRequest editPostRequest)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [HttpPost("report")]
    public async Task<ReportResponse> Report([FromBody] ReportPostRequest reportPostRequest)
    {
        throw new NotImplementedException();
    }
}
