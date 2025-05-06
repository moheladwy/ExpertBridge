using ExpertBridge.Api.Helpers;
using ExpertBridge.Api.Queries;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly AuthorizationHelper _authHelper;

        public NotificationsController(
            ExpertBridgeDbContext dbContext,
            AuthorizationHelper authHelper)
        {
            _dbContext = dbContext;
            _authHelper = authHelper;
        }

        [HttpGet]
        public async Task<List<NotificationResponse>> GetAll()
        {
            var user = await _authHelper.GetCurrentUserAsync();

            if (user == null || user.Profile == null)
            {
                throw new UnauthorizedException();
            }

            var notifications = await _dbContext.Notifications
                .Where(n => n.RecipientId == user.Profile.Id)
                .SelectNotificationResopnse()
                .ToListAsync();

            return notifications;
        }
    }
}
