using ExpertBridge.Api.Services;
using ExpertBridge.Application.Helpers;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     Controller for managing user notifications.
/// </summary>
/// <remarks>
///     All endpoints require authentication.
/// </remarks>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public sealed class NotificationsController : ControllerBase
{
    /// <summary>
    ///     Provides methods to assist with user authorization and retrieval of the currently authenticated user.
    /// </summary>
    private readonly AuthorizationHelper _authHelper;

    /// <summary>
    ///     Provides methods for managing and interacting with user notifications, including retrieval and updates.
    /// </summary>
    private readonly NotificationsService _notificationsService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotificationsController" /> class.
    /// </summary>
    /// <param name="authHelper">The helper for authentication and authorization operations.</param>
    /// <param name="notificationsService">The service for managing notifications.</param>
    public NotificationsController(
        AuthorizationHelper authHelper,
        NotificationsService notificationsService)
    {
        _authHelper = authHelper;
        _notificationsService = notificationsService;
    }

    /// <summary>
    ///     Retrieves all notifications for the current authenticated user.
    /// </summary>
    /// <returns>A list of notifications for the current user.</returns>
    /// <exception cref="UnauthorizedException">Thrown when the user is not authenticated or has no profile.</exception>
    [HttpGet]
    public async Task<List<NotificationResponse>> GetAll()
    {
        var user = await _authHelper.GetCurrentUserAsync();
        if (user?.Profile == null)
        {
            throw new UnauthorizedException();
        }

        return await _notificationsService.GetAll(user);
    }

    /// <summary>
    ///     Marks all unread notifications as read for the current authenticated user.
    /// </summary>
    /// <exception cref="UnauthorizedException">Thrown when the user is not authenticated or has no profile.</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    ///     Thrown when an error is encountered while saving to
    ///     the database.
    /// </exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
    ///     Thrown when a concurrency violation is
    ///     encountered while updating the database.
    /// </exception>
    [HttpPatch]
    public async Task MarkAllRead()
    {
        var user = await _authHelper.GetCurrentUserAsync();
        if (user?.Profile == null)
        {
            throw new UnauthorizedException();
        }

        await _notificationsService.MarkAllRead(user);
    }
}
