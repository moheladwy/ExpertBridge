using Amazon.S3.Model;
using ExpertBridge.Core.DTOs.Responses;

namespace ExpertBridge.Core.Interfaces.Services;

public interface IObjectStorageService
{
    Task<GetFileResponse> GetObjectAsync(string key);
    Task<string> GetObjectUrlAsync(string key);
    Task<string> GetPresignedUrlAsync(string key);
    Task<UploadFileResponse> UploadObjectAsync(PutObjectRequest request);
    Task DeleteObjectAsync(string key);
}
