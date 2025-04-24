// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using ExpertBridge.Api.Models;

namespace ExpertBridge.Api.Middleware
{
    public class EmailVerifiedMiddleware
    {
        private readonly RequestDelegate _next;

        public EmailVerifiedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var isAuth = context.User?.Identity?.IsAuthenticated ?? false;

            if (isAuth)
            {
                bool isEmailVerified = context.User.FindFirstValue("email_verified") == "true";
                if (!isEmailVerified)
                {
                    throw new UnauthorizedException("Email not verified");
                }
            }

            await _next(context);
        }
    }
}
