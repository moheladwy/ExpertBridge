// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Responses;

namespace ExpertBridge.Api.Models.GroqResponses;

public class TranslateTagsResponse
{
    public List<CategorizerTagResponse> Tags { get; set; }
}
