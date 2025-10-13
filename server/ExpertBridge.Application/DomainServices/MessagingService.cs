using ExpertBridge.Core.Entities.Messages;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpertBridge.Application.DomainServices;

public class MessagingService
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly IHubContext<NotificationsHub, INotificationClient> _hubContext;
    private readonly ILogger<MessagingService> _logger;
    private readonly NotificationFacade _notifications;

    public MessagingService(
        ExpertBridgeDbContext dbContext,
        IHubContext<NotificationsHub, INotificationClient> hubContext,
        NotificationFacade notifications,
        ILogger<MessagingService> logger)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _notifications = notifications;
        _logger = logger;
    }

    public async Task<MessageResponse> CreateAsync(Profile userProfile, CreateMessageRequest request)
    {
        ArgumentNullException.ThrowIfNull(userProfile);
        ArgumentNullException.ThrowIfNull(request);

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
