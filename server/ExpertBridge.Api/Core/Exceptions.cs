// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core;

public class UserNotFoundException(string message) : Exception(message);

public class ProfileNotFoundException(string message) : Exception(message);

public class PostNotFoundException(string message) : Exception(message);

public class MediaNotFoundException(string message) : Exception(message);

public class CommentNotFoundException(string message) : Exception(message);

public class UnauthorizedException : Exception
{
    public UnauthorizedException() { }
    public UnauthorizedException(string message) : base(message) { }
    public UnauthorizedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
