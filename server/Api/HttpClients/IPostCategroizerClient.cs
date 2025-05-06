// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Core.Requests;
using Core.Responses;
using Refit;

namespace Api.HttpClients
{
    public interface IPostCategroizerClient
    {
        [Headers("Content-Type: application/json")]
        [Post("/categorize")]
        Task<ApiResponse<PostCategorizerResponse>> GetPostTagsAsync([Body] PostCategorizerRequest request);
    }
}
