// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Responses
{
    public class MediaObjectResponse
    {
        public string Id { get; set; }
        public required string Name { get; set; }
        public required string Url { get; set; }
        public required string Type { get; set; }
        public string Key { get; set; }
    }
}
