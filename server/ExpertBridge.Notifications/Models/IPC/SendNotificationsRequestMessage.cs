// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Notifications.Models.IPC;

public class SendNotificationsRequestMessage
{
    public List<SendNotificationMessage> Notifications { get; set; } = new();
}
