using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

namespace ExpertBridge.Application.Helpers;

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
