// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.AI;
using Pgvector;

namespace ExpertBridge.Api.EmbeddingService;

public class OllamaEmbeddingService(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator
    ) : IEmbeddingService
{
    public async Task<Vector> GenerateEmbedding(string text)
    {
        var generatedEmbeddings = await embeddingGenerator.GenerateAsync([text]);
        var embedding = generatedEmbeddings.Single();
        return new Vector(embedding.Vector);
    }
}
