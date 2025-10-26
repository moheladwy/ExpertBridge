using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

namespace ExpertBridge.Application.Helpers;

/// <summary>
///     Provides extension methods for converting ASP.NET Core file uploads to AWS S3 request objects.
/// </summary>
/// <remarks>
///     This static helper class simplifies the process of preparing uploaded files for storage in AWS S3.
///     It handles the common conversion pattern from IFormFile (ASP.NET Core) to PutObjectRequest (AWS SDK).
///     **Key Responsibilities:**
///     - Convert IFormFile to S3-compatible request objects
///     - Handle stream management for file uploads
///     - Preserve file metadata during conversion
///     - Ensure proper memory stream positioning
///     **Use Cases:**
///     - Profile picture uploads
///     - Post media attachments
///     - Document uploads
///     - Any file upload to S3 storage
///     **Integration with S3Service:**
///     This helper works in conjunction with S3Service to streamline the file upload workflow:
///     1. Client uploads file via multipart/form-data
///     2. ASP.NET Core binds to IFormFile
///     3. MediaHelper converts IFormFile to PutObjectRequest
///     4. S3Service uses request to upload to S3 bucket
///     **Design Pattern:**
///     Extension method pattern provides fluent syntax for file conversion:
///     <code>
/// var putRequest = await formFile.ToPutObjectRequestAsync();
/// await s3Client.PutObjectAsync(putRequest);
/// </code>
///     **Memory Management:**
///     The method creates a MemoryStream that should be disposed by the caller
///     or handled by the AWS SDK's disposal patterns.
///     Internal visibility ensures this helper is only used within the Application layer.
/// </remarks>
internal static class MediaHelper
{
    /// <summary>
    ///     Converts an ASP.NET Core IFormFile to an AWS S3 PutObjectRequest for file upload operations.
    /// </summary>
    /// <param name="file">
    ///     The uploaded file from an ASP.NET Core form. Must not be null.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing a <see cref="PutObjectRequest" />
    ///     configured with the file's content, content type, and metadata.
    /// </returns>
    /// <remarks>
    ///     This extension method performs the following operations:
    ///     **1. Stream Creation:**
    ///     - Creates a new MemoryStream to hold file contents
    ///     - Copies IFormFile content to MemoryStream asynchronously
    ///     - Resets stream position to 0 for reading by AWS SDK
    ///     **2. Request Configuration:**
    ///     - InputStream: Set to the MemoryStream containing file data
    ///     - ContentType: Copied from IFormFile (e.g., "image/jpeg", "application/pdf")
    ///     - Metadata: Includes original filename for reference
    ///     **3. Metadata Preservation:**
    ///     The "file-name" metadata allows:
    ///     - Original filename retrieval from S3
    ///     - File type identification
    ///     - Download filename suggestions
    ///     - Audit and debugging capabilities
    ///     **Stream Positioning:**
    ///     Critical: Stream position is reset to 0 after copying.
    ///     This ensures AWS SDK can read from the beginning of the stream.
    ///     Without this, uploads would be empty or fail.
    ///     **Content-Type Handling:**
    ///     The ContentType is automatically detected by ASP.NET Core based on file extension.
    ///     Common content types:
    ///     - Images: image/jpeg, image/png, image/gif
    ///     - Documents: application/pdf, application/msword
    ///     - Videos: video/mp4, video/quicktime
    ///     **Example Usage:**
    ///     <code>
    /// [HttpPost("upload")]
    /// public async Task&lt;IActionResult&gt; UploadFile(IFormFile file)
    /// {
    ///     if (file == null || file.Length == 0)
    ///         return BadRequest("No file uploaded");
    ///     
    ///     var putRequest = await file.ToPutObjectRequestAsync();
    ///     putRequest.BucketName = "my-bucket";
    ///     putRequest.Key = Guid.NewGuid().ToString();
    ///     
    ///     await s3Client.PutObjectAsync(putRequest);
    ///     return Ok(new { fileId = putRequest.Key });
    /// }
    /// </code>
    ///     **Additional Configuration Needed:**
    ///     The returned PutObjectRequest still requires:
    ///     - BucketName: Target S3 bucket
    ///     - Key: Unique object identifier in S3
    ///     - Optional: ACL, ServerSideEncryption, StorageClass
    ///     These are typically set by the calling service (e.g., S3Service).
    ///     **Memory Considerations:**
    ///     - MemoryStream loads entire file into memory
    ///     - Not suitable for very large files (>100MB)
    ///     - For large files, consider streaming directly from IFormFile.OpenReadStream()
    ///     - MemoryStream will be disposed when PutObjectRequest is disposed
    ///     **Performance:**
    ///     - Asynchronous copy prevents thread blocking
    ///     - Memory stream allows multiple read operations if needed
    ///     - Efficient for typical file sizes (KB to few MB)
    ///     **Validation:**
    ///     The method assumes the file has already been validated:
    ///     - File size limits
    ///     - Content type restrictions
    ///     - File extension validation
    ///     - Malware scanning
    ///     Perform these validations before calling this method.
    ///     **Error Handling:**
    ///     May throw:
    ///     - ArgumentNullException if file is null
    ///     - IOException if stream operations fail
    ///     - OutOfMemoryException for extremely large files
    ///     Caller should handle exceptions appropriately.
    /// </remarks>
    public static async Task<PutObjectRequest> ToPutObjectRequestAsync(this IFormFile file)
    {
        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var putRequest = new PutObjectRequest
        {
            InputStream = memoryStream, ContentType = file.ContentType, Metadata = { ["file-name"] = file.FileName }
        };
        return putRequest;
    }
}
