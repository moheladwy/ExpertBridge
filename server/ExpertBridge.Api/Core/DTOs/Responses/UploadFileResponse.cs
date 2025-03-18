// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.DTOs.Responses;

public class UploadFileResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string FileUrl { get; set; }
}
