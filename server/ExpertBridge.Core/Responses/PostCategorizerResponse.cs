// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Responses
{
    public class PostCategorizerResponse
    {
        public string Language { get; set; }
        public List<CategorizerTag> Tags { get; set; }
    }

    public class CategorizerTag
    {
        public string Tag { get; set; }
        public string Description { get; set; }
    }
}
