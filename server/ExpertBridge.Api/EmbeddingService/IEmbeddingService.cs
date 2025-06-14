// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Pgvector;

namespace ExpertBridge.Api.EmbeddingService;

public interface IEmbeddingService
{
    Task<Vector> GenerateEmbedding(string text);
}
