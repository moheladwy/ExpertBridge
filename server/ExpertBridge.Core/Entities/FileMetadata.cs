// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Entities
{
    public class FileMetadata
    {
        public string ContentType { get; set; }
        public string? Key { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string Extension { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
