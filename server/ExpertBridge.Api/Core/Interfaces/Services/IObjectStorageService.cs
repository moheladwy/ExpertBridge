// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Amazon.S3.Model;
using ExpertBridge.Api.Core.DTOs.Responses;

namespace ExpertBridge.Api.Core.Interfaces.Services;

public interface IObjectStorageService
{
    Task<GetFileResponse> GetObjectAsync(string key);
    Task<GetMediaUrlResponse> GetObjectUrlAsync(string key);
    Task<GetMediaUrlResponse> GetPresignedUrlAsync(string key);
    Task<UploadFileResponse> UploadObjectAsync(PutObjectRequest request);
    Task DeleteObjectAsync(string key);
}
