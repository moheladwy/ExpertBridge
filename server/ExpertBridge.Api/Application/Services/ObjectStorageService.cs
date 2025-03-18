// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amazon.S3;
using Amazon.S3.Model;
using ExpertBridge.Api.Configurations;
using ExpertBridge.Api.Core.DTOs.Responses;
using ExpertBridge.Api.Core.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Api.Application.Services;

public class ObjectStorageService(
    IAmazonS3 s3Client,
    IOptions<AwsConfigurations> awsConfigurations
    ) : IObjectStorageService
{
    public async Task<GetFileResponse> GetObjectAsync(string key)
    {
        var request = new GetObjectRequest
        {
            BucketName = awsConfigurations.Value.BucketName,
            Key = key
        };
        var response = await s3Client.GetObjectAsync(request);
        var fileResponse = new GetFileResponse
        {
            ResponseStream = response.ResponseStream,
            ContentType = response.Headers.ContentType,
            FileName = response.Key
        };

        return fileResponse;
    }

    public async Task<GetMediaUrlResponse> GetObjectUrlAsync(string key)
    {
        return await Task.FromResult(
            new GetMediaUrlResponse
            {
                Url = $"https://{awsConfigurations.Value.BucketName}.s3.amazonaws.com/{key}"
            }
            );
    }

    public async Task<GetMediaUrlResponse> GetPresignedUrlAsync(string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = awsConfigurations.Value.BucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(60)
        };

        return new GetMediaUrlResponse { Url = await s3Client.GetPreSignedURLAsync(request) };
    }

    public async Task<UploadFileResponse> UploadObjectAsync(PutObjectRequest request)
    {
        request.Key = Guid.NewGuid().ToString();
        request.BucketName = awsConfigurations.Value.BucketName;
        var response = await s3Client.PutObjectAsync(request);
        var fileUrl = await GetPresignedUrlAsync(request.Key);

        return new UploadFileResponse
        {
            StatusCode = (int)response.HttpStatusCode,
            Message = $"File with name: `{request.Metadata["file-name"]}` uploaded successfully!",
            FileUrl = fileUrl.Url
        };
    }

    public async Task DeleteObjectAsync(string key)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = awsConfigurations.Value.BucketName,
            Key = key
        };
        await s3Client.DeleteObjectAsync(request);
    }
}
