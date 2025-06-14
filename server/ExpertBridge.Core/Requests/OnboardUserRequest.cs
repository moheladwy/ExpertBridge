// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Requests
{
    public class OnboardUserRequest
    {
        public List<string> TagIds { get; set; }
    }

    public class OnboardUserRequestV2
    {
        public List<string> Tags { get; set; }
    }
}
