// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Exceptions;

public abstract class HttpNotFoundException : Exception
{
    public HttpNotFoundException() { }
    public HttpNotFoundException(string message) : base(message) { }
    public HttpNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class UserNotFoundException : HttpNotFoundException
{
    public UserNotFoundException() { }
    public UserNotFoundException(string message) : base(message) { }
    public UserNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class JobNotFoundException : HttpNotFoundException
{
    public JobNotFoundException() { }
    public JobNotFoundException(string message) : base(message) { }
    public JobNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class ChatNotFoundException : HttpNotFoundException
{
    public ChatNotFoundException() { }
    public ChatNotFoundException(string message) : base(message) { }
    public ChatNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class JobOfferNotFoundException : HttpNotFoundException
{
    public JobOfferNotFoundException() { }
    public JobOfferNotFoundException(string message) : base(message) { }
    public JobOfferNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class JobApplicationNotFoundException : HttpNotFoundException
{
    public JobApplicationNotFoundException() { }
    public JobApplicationNotFoundException(string message) : base(message) { }
    public JobApplicationNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class ProfileNotFoundException : HttpNotFoundException
{
    public ProfileNotFoundException() { }
    public ProfileNotFoundException(string message) : base(message) { }
    public ProfileNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class PostNotFoundException : HttpNotFoundException
{
    public PostNotFoundException() { }
    public PostNotFoundException(string message) : base(message) { }
    public PostNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class MediaNotFoundException : HttpNotFoundException
{
    public MediaNotFoundException() { }
    public MediaNotFoundException(string message) : base(message) { }
    public MediaNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class CommentNotFoundException : HttpNotFoundException
{
    public CommentNotFoundException() { }
    public CommentNotFoundException(string message) : base(message) { }
    public CommentNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException() { }
    public UnauthorizedException(string message) : base(message) { }
    public UnauthorizedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class UnauthorizedGetMyProfileException : UnauthorizedException
{
    public UnauthorizedGetMyProfileException() { }
    public UnauthorizedGetMyProfileException(string message) : base(message) { }
    public UnauthorizedGetMyProfileException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}


public class RemoteServiceCallFailedException : Exception
{
    public RemoteServiceCallFailedException() { }
    public RemoteServiceCallFailedException(string message) : base(message) { }
    public RemoteServiceCallFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() { }
    public ForbiddenAccessException(string message) : base(message) { }
    public ForbiddenAccessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class ProfileUserNameAlreadyExistsException : Exception
{
    public ProfileUserNameAlreadyExistsException() { }
    public ProfileUserNameAlreadyExistsException(string message) : base(message) { }
    public ProfileUserNameAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class ProfilePhoneNumberAlreadyExistsException : Exception
{
    public ProfilePhoneNumberAlreadyExistsException() { }
    public ProfilePhoneNumberAlreadyExistsException(string? message) : base(message) { }
    public ProfilePhoneNumberAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException) { }
}

