using ExpertBridge.Api.Services;
using ExpertBridge.Application.DomainServices;
using ExpertBridge.Contract.Requests.CreateMessage;
using ExpertBridge.Contract.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     Controller for managing chat messages in the system.
/// </summary>
/// <remarks>
///     This controller provides endpoints to retrieve chat messages and create new messages.
///     It ensures that all requests are authorized and operates on behalf of the currently authenticated user.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MessagesController : ControllerBase
{
    /// <summary>
    ///     Provides access to the messaging service for handling message-related operations within the controller.
    /// </summary>
    private readonly MessagingService _messagingService;

    /// <summary>
    ///     Responsible for managing operations and interactions related to user data within the controller.
    /// </summary>
    private readonly UserService _userService;

    /// <summary>
    ///     The MessagesController is responsible for handling HTTP requests related to messaging functionality within the API.
    ///     It provides routing and authorization features for endpoints managing messages and related operations.
    /// </summary>
    public MessagesController(
        UserService userService,
        MessagingService messagingService)
    {
        _userService = userService;
        _messagingService = messagingService;
    }

    /// <summary>
    ///     Retrieves a list of messages from a specified chat given the chat identifier.
    ///     This method ensures the user is authorized and retrieves messages for the
    ///     currently authenticated user.
    /// </summary>
    /// <param name="chatId">The unique identifier of the chat whose messages are to be retrieved.</param>
    /// <returns>A task that resolves to a list of MessageResponse objects representing the chat messages.</returns>
    [HttpGet]
    public async Task<List<MessageResponse>> GetChatMessages([FromQuery] string chatId)
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var messages = await _messagingService.GetChatMessagesAsync(userProfile, chatId);

        return messages;
    }

    /// <summary>
    ///     Handles the creation of a new message. This method processes the incoming message request,
    ///     validates the current user's profile, and produces a response object containing the newly created message details.
    /// </summary>
    /// <param name="request">The request object containing the data necessary to create a new message.</param>
    /// <returns>A <see cref="MessageResponse" /> containing the details of the created message.</returns>
    [HttpPost]
    public async Task<MessageResponse> CreateMessage([FromBody] CreateMessageRequest request)
    {
        var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();

        var message = await _messagingService.CreateAsync(userProfile, request);

        return message;
    }
}
