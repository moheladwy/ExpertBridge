// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amazon.S3;
using Amazon.S3.Model;
using ExpertBridge.Api.Models;
using ExpertBridge.Api.Responses;
using ExpertBridge.Api.Settings;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Api.Services;

public class S3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly AwsSettings _awsSettings;

    public S3Service(
        IAmazonS3 s3Client,
        IOptionsSnapshot<AwsSettings> awsSettings)
    {
        _s3Client = s3Client;
        _awsSettings = awsSettings.Value;
    }

    public async Task<PresignedUrlResponse> GetPresignedPutUrlAsync(FileMetadata file)
    {
        ArgumentNullException.ThrowIfNull(file, nameof(file));

        if (file.Size > _awsSettings.MaxFileSize)
        {
            throw new InvalidOperationException($"File size exceeds the maximum allowed size of {_awsSettings.MaxFileSize} bytes.");
        }

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _awsSettings.BucketName,
            Key = Guid.NewGuid().ToString(),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Verb = HttpVerb.PUT,
            Headers =
            {
                ["Cache-Control"] = _awsSettings.CacheControl
            }
        };

        request.Metadata.Add("file-name", file.Name);
        request.Metadata.Add("file-size", file.Size.ToString());
        request.Metadata.Add("file-type", file.Type);
        request.Metadata.Add("file-extension", file.Extension);
        request.Metadata.Add("file-key", request.Key);
        request.Metadata.Add("max-size", _awsSettings.MaxFileSize.ToString());
        request.Metadata.Add("cache-control", _awsSettings.CacheControl);

        var response = new PresignedUrlResponse
        {
            Url = await _s3Client.GetPreSignedURLAsync(request),
            Key = request.Key
        };

        return response;
    }

    public async Task<PresignedUrlResponse> GetPresignedGetUrlAsync(string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _awsSettings.BucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(60)
        };

        var response = new PresignedUrlResponse
        {
            Url = await _s3Client.GetPreSignedURLAsync(request),
            Key = key
        };

        return response;
    }

    //public async Task<GetFileResponse> GetObjectAsync(string key)
    //{
    //    var request = new GetObjectRequest
    //    {
    //        BucketName = awsSettings.Value.BucketName,
    //        Key = key
    //    };
    //    var response = await s3Client.GetObjectAsync(request);
    //    var fileResponse = new GetFileResponse
    //    {
    //        ResponseStream = response.ResponseStream,
    //        ContentType = response.Headers.ContentType,
    //        FileName = response.Key
    //    };

    //    return fileResponse;
    //}

    public string GetObjectUrl(string key)
    {
        return $"https://{_awsSettings.BucketName}.s3.amazonaws.com/{key}";
    }

    //public async Task<UploadFileResponse> UploadObjectAsync(PutObjectRequest request)
    //{
    //    request.Key = Guid.NewGuid().ToString();
    //    request.BucketName = awsSettings.Value.BucketName;
    //    var response = await s3Client.PutObjectAsync(request);
    //    var fileUrl = await GetPresignedUrlAsync(request.Key);

    //    return new UploadFileResponse
    //    {
    //        StatusCode = (int)response.HttpStatusCode,
    //        Message = $"File with name: `{request.Metadata["file-name"]}` uploaded successfully!",
    //        FileUrl = fileUrl.Url
    //    };
    //}

    public async Task DeleteObjectAsync(string key)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _awsSettings.BucketName,
            Key = key
        };

        await _s3Client.DeleteObjectAsync(request);
    }
}
