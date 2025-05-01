// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Settings
{
    public class EmbeddingServiceSettings
    {
        public const string Section = "Ollama";

        public string Url { get; set; }
        public string ModelId { get; set; }
    }
}
