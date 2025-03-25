// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using ExpertBridge.Api.Core.Entities.Users;
using ExpertBridge.Api.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Helpers
{
    public class AuthorizationHelper
    {
        private readonly ExpertBridgeDbContext _dbContext;

        public AuthorizationHelper(ExpertBridgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal? claims)
        {
            string? email = claims?.FindFirstValue(ClaimTypes.Email);

            var user = await _dbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
    }
}
