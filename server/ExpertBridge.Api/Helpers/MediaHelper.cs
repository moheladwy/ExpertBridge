// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amazon.S3.Model;

namespace ExpertBridge.Api.Helpers;

internal static class MediaHelper
{
    public static async Task<PutObjectRequest> ToPutObjectRequestAsync(this IFormFile file)
    {
        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var putRequest = new PutObjectRequest
        {
            InputStream = memoryStream,
            ContentType = file.ContentType,
            Metadata =
            {
                ["file-name"] = file.FileName
            }
        };
        return putRequest;
    }
}
