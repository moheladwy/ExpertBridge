// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Pgvector;

namespace ExpertBridge.Core.Interfaces;

public interface IRecommendableContent
{
    string Id { get; set; }
    string AuthorId { get; set; }
    string Title { get; set; }
    string Content { get; set; }
    string? Language { get; set; }
    bool IsProcessed { get; set; }
    bool IsTagged { get; set; }
    Vector? Embedding { get; set; }
}
