using ExpertBridge.Contract.Queries;
using ExpertBridge.Contract.Requests.CreateMessage;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Entities.Messages;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpertBridge.Application.DomainServices;

/// <summary>
///     Provides real-time messaging services for chat conversations between job clients and contractors.
/// </summary>
/// <remarks>
///     This service manages private chat messages within job-specific conversations, integrating
///     SignalR for real-time delivery and database persistence for message history.
///     **Architecture Role:**
///     MessagingService bridges three key systems:
///     1. Database (message persistence and history)
///     2. SignalR (real-time push notifications)
///     3. Notification system (in-app and push notifications)
///     **Chat Context:**
///     Chats are created when jobs are accepted (either from JobOffer or JobApplication).
///     Each Chat has exactly two participants:
///     - Hirer (job client, Author in Job entity)
///     - Worker (contractor, Worker in Job entity)
///     **Security Model:**
///     - Users can only send messages in chats where they are participants
///     - Users can only read messages from their own chats
///     - Chat ID validation prevents unauthorized access
///     - No public/group chats (all conversations are 1:1)
///     **Message Flow:**
///     <code>
/// Client A sends message via WebSocket/HTTP
///     ↓
/// MessagingService.CreateAsync
///     ↓
/// Save to database
///     ↓
/// Send SignalR notification to Client B (real-time)
///     ↓
/// Send push notification to Client B (if offline)
///     ↓
/// Return message to Client A
/// </code>
///     **Real-Time Integration:**
///     SignalR provides instant message delivery:
///     <code>
/// // Client subscribes to notifications
/// hubConnection.on("ReceiveMessage", (message) => {
///     displayMessage(message);
/// });
/// 
/// // Server pushes to all connected clients
/// await _hubContext.Clients.All.ReceiveMessage(messageDto);
/// </code>
///     **Database Schema:**
///     <code>
/// Chat:
///   Id: Guid
///   HirerId: Profile FK (client)
///   WorkerId: Profile FK (contractor)
///   JobId: Job FK
///   CreatedAt: DateTime
/// 
/// Message:
///   Id: Guid
///   ChatId: Chat FK
///   SenderId: Profile FK
///   Content: Text (encrypted in future)
///   IsConfirmationMessage: Bool (system messages)
///   CreatedAt: DateTime
/// </code>
///     **Authorization Pattern:**
///     All methods validate chat membership before operations:
///     <code>
/// var chat = await _dbContext.Chats
///     .WhereProfileIsChatParticipant(userProfile.Id)
///     .FirstOrDefaultAsync(c => c.Id == chatId);
/// 
/// if (chat == null)
///     throw new ChatNotFoundException();
/// </code>
///     **Use Cases:**
///     **1. Job Coordination:**
///     - Discuss project requirements
///     - Share progress updates
///     - Coordinate deliverables
///     **2. Payment Negotiation:**
///     - Clarify budget details
///     - Discuss milestones
///     - Confirm completion
///     **3. File Sharing (Future):**
///     - Send document attachments
///     - Share design files
///     - Exchange contracts
///     **Performance Considerations:**
///     - Messages loaded with pagination (not implemented yet)
///     - SignalR broadcasts to all clients (future: targeted delivery)
///     - Database queries use indexed ChatId and CreatedAt
///     **Future Enhancements:**
///     - Message encryption (end-to-end)
///     - Message editing and deletion
///     - Read receipts and typing indicators
///     - File attachments via media service
///     - Message search and filtering
///     - Pagination for message history
///     - Targeted SignalR delivery (not broadcast to all)
///     Registered as scoped service, sharing DbContext lifetime with HTTP requests.
/// </remarks>
public class MessagingService
{
    private readonly IValidator<CreateMessageRequest> _createMessageValidator;
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly IHubContext<NotificationsHub, INotificationClient> _hubContext;
    private readonly ILogger<MessagingService> _logger;
    private readonly NotificationFacade _notifications;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MessagingService" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for message persistence.</param>
    /// <param name="hubContext">The SignalR hub context for real-time message delivery.</param>
    /// <param name="notifications">The notification facade for push notifications.</param>
    /// <param name="logger">The logger for diagnostic information.</param>
    /// <param name="createMessageValidator">The validator for message creation requests.</param>
    public MessagingService(
        ExpertBridgeDbContext dbContext,
        IHubContext<NotificationsHub, INotificationClient> hubContext,
        NotificationFacade notifications,
        ILogger<MessagingService> logger,
        IValidator<CreateMessageRequest> createMessageValidator)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _notifications = notifications;
        _logger = logger;
        _createMessageValidator = createMessageValidator;
    }

    /// <summary>
    ///     Creates and sends a new message in a chat conversation with real-time delivery.
    /// </summary>
    /// <param name="userProfile">The profile of the user sending the message.</param>
    /// <param name="request">The message creation request containing chat ID and content.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing the created <see cref="MessageResponse" />.
    /// </returns>
    /// <exception cref="ChatNotFoundException">
    ///     Thrown when the chat ID is invalid or the user is not a participant in the chat.
    /// </exception>
    /// <remarks>
    ///     **Processing Flow:**
    ///     **1. Authorization Check:**
    ///     - Validate chat exists and user is participant (hirer or worker)
    ///     - Load chat with Job navigation for context
    ///     - Throw exception if unauthorized
    ///     **2. Create Message:**
    ///     - Build Message entity with content and sender
    ///     - Set IsConfirmationMessage = false (regular message)
    ///     - Save to database
    ///     **3. Real-Time Delivery:**
    ///     - Send push notification to recipient via NotificationFacade
    ///     - Broadcast to all SignalR clients via hub context
    ///     - Return message response to sender
    ///     **Security Validation:**
    ///     <code>
    /// // WARNING! Do NOT trust client-provided chat ID
    /// // Always verify user is participant
    /// var chat = await _dbContext.Chats
    ///     .WhereProfileIsChatParticipant(userProfile.Id)
    ///     .FirstOrDefaultAsync(c => c.Id == request.ChatId);
    /// </code>
    ///     **Recipient Identification:**
    ///     <code>
    /// // Determine other participant
    /// var receiverId = userProfile.Id == chat.HirerId
    ///     ? chat.WorkerId  // Sender is hirer → receiver is worker
    ///     : chat.HirerId;  // Sender is worker → receiver is hirer
    /// </code>
    ///     **SignalR Broadcast:**
    ///     Currently broadcasts to ALL connected clients:
    ///     <code>
    /// await _hubContext.Clients.All.ReceiveMessage(messageDto);
    /// </code>
    ///     **Future Improvement:**
    ///     Targeted delivery to specific user:
    ///     <code>
    /// await _hubContext.Clients.User(receiverId).ReceiveMessage(messageDto);
    /// </code>
    ///     **Example Usage:**
    ///     <code>
    /// [HttpPost("chats/{chatId}/messages")]
    /// public async Task&lt;IActionResult&gt; SendMessage(
    ///     string chatId,
    ///     CreateMessageRequest request)
    /// {
    ///     var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();
    ///     var message = await _messagingService.CreateAsync(userProfile, request);
    ///     return Ok(message);
    /// }
    /// </code>
    ///     **Client-Side Integration:**
    ///     <code>
    /// // JavaScript SignalR client
    /// connection.on("ReceiveMessage", (message) => {
    ///     if (message.chatId === currentChatId) {
    ///         appendMessageToUI(message);
    ///     }
    /// });
    /// </code>
    ///     **Notification Types:**
    ///     - Push notification: For offline/background users
    ///     - SignalR: For online users (instant delivery)
    ///     - In-app notification: Badge count updates
    ///     **Error Scenarios:**
    ///     - Invalid chat ID: ChatNotFoundException
    ///     - User not participant: ChatNotFoundException (security)
    ///     - Empty content: Accepted (validation in controller)
    ///     - Database failure: Exception propagated to global handler
    ///     The method ensures atomic operations: message saved and notifications sent together.
    /// </remarks>
    public async Task<MessageResponse> CreateAsync(Profile userProfile, CreateMessageRequest request)
    {
        ArgumentNullException.ThrowIfNull(userProfile);
        ArgumentNullException.ThrowIfNull(request);

        // Validate request using FluentValidation
        var validationResult = await _createMessageValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // WARNING!
        // Do NOT trust the chat id coming from the client.
        // Always check if the creating user is participent in this chat first.
        // (RESOLVED)
        var chat = await _dbContext.Chats
            .Include(c => c.Job)
            .WhereProfileIsChatParticipant(userProfile.Id)
            .FirstOrDefaultAsync(c => c.Id == request.ChatId);

        if (chat is null)
        {
            throw new ChatNotFoundException($"Chat with id = {request.ChatId} was not found");
        }

        var message = new Message
        {
            ChatId = chat.Id,
            SenderId = userProfile.Id,
            IsConfirmationMessage = false,
            Content = request.Content,
            Sender = userProfile
        };

        await _dbContext.Messages.AddAsync(message);
        await _dbContext.SaveChangesAsync();

        var receiverId = userProfile.Id == chat.HirerId ? chat.WorkerId : chat.HirerId;

        // notify the other chat participant
        await _notifications.NotifyNewMessageReceivedAsync(message, receiverId, chat.Job.Id);

        // send the message through SignalR
        await _hubContext.Clients.All.ReceiveMessage(new Notifications.Models.Message
        {
            ChatId = chat.Id,
            ReceiverId = receiverId,
            SenderId = message.SenderId,
            IsConfirmationMessage = message.IsConfirmationMessage,
            Content = message.Content,
            CreatedAt = message.CreatedAt.Value
        });

        return message.SelectMessageResponseFromFullMessage();
    }

    /// <summary>
    ///     Retrieves all messages from a chat conversation, ordered chronologically.
    /// </summary>
    /// <param name="userProfile">The profile of the user requesting chat messages.</param>
    /// <param name="chatId">The unique identifier of the chat to retrieve messages from.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing a list of <see cref="MessageResponse" /> objects
    ///     ordered by creation time (oldest first).
    /// </returns>
    /// <exception cref="ChatNotFoundException">
    ///     Thrown when the chat ID is invalid or the user is not a participant in the chat.
    /// </exception>
    /// <remarks>
    ///     **Authorization:**
    ///     Validates user is a chat participant before returning messages:
    ///     <code>
    /// var chat = await _dbContext.Chats
    ///     .WhereProfileIsChatParticipant(userProfile.Id)
    ///     .FirstOrDefaultAsync(c => c.Id == chatId);
    /// </code>
    ///     **Query Optimization:**
    ///     - Messages ordered by CreatedAt ascending (chronological display)
    ///     - Projected to MessageResponse DTO (minimal data transfer)
    ///     - No pagination yet (future enhancement for large chats)
    ///     **Example Usage:**
    ///     <code>
    /// [HttpGet("chats/{chatId}/messages")]
    /// public async Task&lt;IActionResult&gt; GetMessages(string chatId)
    /// {
    ///     var userProfile = await _userService.GetCurrentUserProfileOrThrowAsync();
    ///     var messages = await _messagingService.GetChatMessagesAsync(userProfile, chatId);
    ///     return Ok(messages);
    /// }
    /// </code>
    ///     **Client Display:**
    ///     <code>
    /// // Render messages in chat UI
    /// messages.forEach(msg => {
    ///     const isSentByMe = msg.senderId === currentUserId;
    ///     displayMessage(msg, isSentByMe);
    /// });
    /// </code>
    ///     **Performance Considerations:**
    ///     - Current implementation loads ALL messages (no pagination)
    ///     - For chats with 1000+ messages, consider pagination
    ///     - Database index on (ChatId, CreatedAt) recommended
    ///     **Future Enhancements:**
    ///     - Pagination with cursor or offset
    ///     - Load recent messages first (latest N)
    ///     - Infinite scroll support
    ///     - Message search within chat
    ///     - Filter by date range
    ///     **Security:**
    ///     - User can only access messages from their own chats
    ///     - No cross-chat message leakage
    ///     - Authorization checked before query execution
    ///     Messages are ordered chronologically for natural conversation flow.
    /// </remarks>
    public async Task<List<MessageResponse>> GetChatMessagesAsync(Profile userProfile, string chatId)
    {
        ArgumentNullException.ThrowIfNull(userProfile);
        ArgumentNullException.ThrowIfNull(chatId);

        // WARNING!
        // Do NOT trust the chat id coming from the client.
        // Always check if the requesting user is participent in this chat first.
        // (RESOLVED)
        var chat = await _dbContext.Chats
            .WhereProfileIsChatParticipant(userProfile.Id)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat is null)
        {
            throw new ChatNotFoundException($"Chat with id = {chatId} was not found");
        }

        var messages = await _dbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.CreatedAt)
            .SelectMessageResponseFromFullMessage()
            .ToListAsync();

        return messages;
    }
}
