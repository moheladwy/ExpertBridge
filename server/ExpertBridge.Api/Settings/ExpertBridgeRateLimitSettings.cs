// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Settings
{
    public class ExpertBridgeRateLimitSettings
    {
        public const string SectionName = "RateLimit";

        public int PermitLimit { get; set; }
        public int Window { get; set; }
        public int QueueLimit { get; set; }
    }
}
