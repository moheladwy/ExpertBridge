// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Core.Entities.Media.MediaGrants
{
    public class MediaGrant : ISoftDeletable
    {
        public int Id { get; set; }
        public required string Key { get; set; }
        public bool OnHold { get; set; }
        public bool IsActive { get; set; }
        public DateTime GrantedAt { get; set; }
        public DateTime? ActivatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
