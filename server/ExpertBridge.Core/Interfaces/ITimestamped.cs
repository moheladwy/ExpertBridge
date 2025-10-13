// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Interfaces;

public interface ITimestamped
{
    DateTime? CreatedAt { get; set; }
    DateTime? LastModified { get; set; }
}
