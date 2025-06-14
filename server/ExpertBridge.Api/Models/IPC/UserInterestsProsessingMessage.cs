// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Models.IPC;

public class UserInterestsProsessingMessage
{
    public string UserProfileId { get; set; }

    public List<string> InterestsTags { get; set; }
}
