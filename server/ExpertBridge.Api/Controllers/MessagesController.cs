using ExpertBridge.Application.DomainServices;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly MessagingService _messagingService;

        public MessagesController(
            UserService userService,
            MessagingService messagingService)
        {
            _userService = userService;
            _messagingService = messagingService;
        }

        [HttpGet]
        public async Task<List<MessageResponse>> GetChatMessages([FromQuery] string chatId)
        {
            var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

            var messages = await _messagingService.GetChatMessagesAsync(userProfile, chatId);

            return messages;
        }

        [HttpPost]
        public async Task<MessageResponse> CreateMessage([FromBody] CreateMessageRequest request)
        {
            var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

            var message = await _messagingService.CreateAsync(userProfile, request);

            return message;
        }
    }
}
