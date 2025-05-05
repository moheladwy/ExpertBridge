using Amazon.S3;
using Amazon.S3.Model;
using ExpertBridge.Api.Settings;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Responses;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Api.Services;

/// <summary>
///     Provides services for interacting with an S3-compatible storage system, including creating presigned URLs
///     for file uploads and downloads, and managing file objects.
/// </summary>
public sealed class S3Service
{
    /// <summary>
    ///     Represents the configuration settings for AWS services used by the system.
    ///     This includes information such as region, bucket name, access keys, maximum file size, and cache control settings.
    /// </summary>
    private readonly AwsSettings _awsSettings;

    /// <summary>
    ///     Represents the Amazon S3 client used to interact with an S3-compatible storage system.
    ///     This client is responsible for performing operations such as generating presigned URLs,
    ///     uploading and downloading objects, and managing object storage.
    /// </summary>
    private readonly IAmazonS3 _s3Client;

    /// <summary>
    ///     Initializes a new instance of the <see cref="S3Service" /> class.
    ///     Configures the service with the necessary AWS S3 client and settings.
    /// </summary>
    /// <param name="s3Client">
    ///     The Amazon S3 client used to interact with the S3-compatible storage system.
    ///     Provides functionality for generating presigned URLs and managing object storage.
    /// </param>
    /// <param name="awsSettings">
    ///     Configuration settings for AWS services, including region, bucket name,
    ///     access keys, maximum file size, and cache control settings.
    /// </param>
    public S3Service(
        IAmazonS3 s3Client,
        IOptionsSnapshot<AwsSettings> awsSettings)
    {
        _s3Client = s3Client;
        _awsSettings = awsSettings.Value;
    }

    /// <summary>
    ///     Generates a presigned URL for uploading a file to an S3 bucket.
    ///     Validates the file metadata and ensures it adheres to the maximum allowed size.
    ///     Configures the request with metadata and cache control settings for the object to be uploaded.
    /// </summary>
    /// <param name="file">
    ///     Metadata about the file to be uploaded, including its name, size, type, and extension.
    ///     Used to configure the presigned URL and validate file constraints.
    /// </param>
    /// <returns>
    ///     A <see cref="PresignedUrlResponse" /> containing the generated presigned URL and the unique key
    ///     identifying the file in the S3 bucket.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="file" /> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the file size exceeds the maximum allowed size specified in AWS settings.
    /// </exception>
    public async Task<PresignedUrlResponse> GetPresignedPutUrlAsync(FileMetadata file)
    {
        ArgumentNullException.ThrowIfNull(file, nameof(file));

        if (file.Size > _awsSettings.MaxFileSize)
        {
            throw new InvalidOperationException(
                $"File size exceeds the maximum allowed size of {_awsSettings.MaxFileSize} bytes.");
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
            },
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

    /// <summary>
    ///     Generates a presigned GET URL for accessing an object stored in an S3 bucket.
    ///     This URL can be used to retrieve the object without requiring direct authentication.
    /// </summary>
    /// <param name="key">
    ///     The key of the object in the S3 bucket for which the presigned URL is being generated.
    ///     This identifies the file to be accessed.
    /// </param>
    /// <returns>
    ///     A <see cref="PresignedUrlResponse" /> instance containing the generated presigned URL
    ///     and the key of the object.
    /// </returns>
    public async Task<PresignedUrlResponse> GetPresignedGetUrlAsync(string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _awsSettings.BucketName, Key = key, Expires = DateTime.UtcNow.AddMinutes(60)
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

    /// <summary>
    ///     Generates the URL for accessing an object in the S3-compatible storage system.
    ///     The URL points to the specified object in the configured bucket.
    /// </summary>
    /// <param name="key">
    ///     The key of the object in the S3 bucket for which the URL needs to be generated.
    /// </param>
    /// <returns>
    ///     A string containing the URL of the object in the S3-compatible storage system,
    ///     formatted according to the bucket name and key.
    /// </returns>
    public string GetObjectUrl(string key) => $"https://{_awsSettings.BucketName}.s3.amazonaws.com/{key}";

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

    /// <summary>
    ///     Deletes an object from the S3-compatible storage system using the specified key.
    ///     This operation permanently removes the object from the associated bucket.
    /// </summary>
    /// <param name="key">
    ///     The unique identifier of the object to be deleted. This key corresponds to the
    ///     object's location in the bucket.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation of deleting the object.
    /// </returns>
    public async Task DeleteObjectAsync(string key)
    {
        var request = new DeleteObjectRequest { BucketName = _awsSettings.BucketName, Key = key };

        await _s3Client.DeleteObjectAsync(request);
    }
}
