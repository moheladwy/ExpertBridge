// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Interfaces;

namespace ExpertBridge.Core.Entities
{
    public abstract class BaseModel : ITimestamped
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
