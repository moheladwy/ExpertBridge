// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities;

namespace ExpertBridge.Core.Requests;

public class GeneratePresignedUrlsRequest
{
    public List<FileMetadata> Files { get; set; }
}
