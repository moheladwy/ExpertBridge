// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Exceptions;

/// <summary>
///     Represents the base exception for HTTP 404 Not Found errors.
/// </summary>
/// <remarks>
///     This abstract class serves as the foundation for all resource not found exceptions in the ExpertBridge platform.
///     It is handled globally by the exception middleware to return HTTP 404 responses.
/// </remarks>
public abstract class HttpNotFoundException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpNotFoundException" /> class.
    /// </summary>
    public HttpNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpNotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public HttpNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpNotFoundException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public HttpNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when a requested user cannot be found.
/// </summary>
public class UserNotFoundException : HttpNotFoundException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UserNotFoundException" /> class.
    /// </summary>
    public UserNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserNotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UserNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserNotFoundException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UserNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when a requested job cannot be found.
/// </summary>
public class JobNotFoundException : HttpNotFoundException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JobNotFoundException" /> class.
    /// </summary>
    public JobNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="JobNotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public JobNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="JobNotFoundException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public JobNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when a requested chat cannot be found.
/// </summary>
public class ChatNotFoundException : HttpNotFoundException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ChatNotFoundException" /> class.
    /// </summary>
    public ChatNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChatNotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ChatNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChatNotFoundException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ChatNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when a requested job offer cannot be found.
/// </summary>
public class JobOfferNotFoundException : HttpNotFoundException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JobOfferNotFoundException" /> class.
    /// </summary>
    public JobOfferNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="JobOfferNotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public JobOfferNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="JobOfferNotFoundException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public JobOfferNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when a requested job application cannot be found.
/// </summary>
public class JobApplicationNotFoundException : HttpNotFoundException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JobApplicationNotFoundException" /> class.
    /// </summary>
    public JobApplicationNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="JobApplicationNotFoundException" /> class with a specified error
    ///     message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public JobApplicationNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="JobApplicationNotFoundException" /> class with a specified error
    ///     message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public JobApplicationNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when a requested profile cannot be found.
/// </summary>
public class ProfileNotFoundException : HttpNotFoundException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfileNotFoundException" /> class.
    /// </summary>
    public ProfileNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfileNotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ProfileNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfileNotFoundException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ProfileNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when a requested post cannot be found.
/// </summary>
public class PostNotFoundException : HttpNotFoundException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PostNotFoundException" /> class.
    /// </summary>
    public PostNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PostNotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PostNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PostNotFoundException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PostNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when a requested media object cannot be found.
/// </summary>
public class MediaNotFoundException : HttpNotFoundException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MediaNotFoundException" /> class.
    /// </summary>
    public MediaNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MediaNotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MediaNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MediaNotFoundException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MediaNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when a requested comment cannot be found.
/// </summary>
public class CommentNotFoundException : HttpNotFoundException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CommentNotFoundException" /> class.
    /// </summary>
    public CommentNotFoundException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommentNotFoundException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CommentNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommentNotFoundException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CommentNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when an HTTP 401 Unauthorized error occurs.
/// </summary>
/// <remarks>
///     This exception is handled globally by the exception middleware to return HTTP 401 responses.
/// </remarks>
public class UnauthorizedException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedException" /> class.
    /// </summary>
    public UnauthorizedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnauthorizedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnauthorizedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when attempting to access a user's profile without proper authentication.
/// </summary>
public class UnauthorizedGetMyProfileException : UnauthorizedException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedGetMyProfileException" /> class.
    /// </summary>
    public UnauthorizedGetMyProfileException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedGetMyProfileException" /> class with a specified error
    ///     message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnauthorizedGetMyProfileException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedGetMyProfileException" /> class with a specified error
    ///     message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnauthorizedGetMyProfileException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when a call to a remote service or external API fails.
/// </summary>
/// <remarks>
///     This exception is used to wrap failures from external services such as AI/LLM providers, authentication services,
///     or storage services.
/// </remarks>
public class RemoteServiceCallFailedException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RemoteServiceCallFailedException" /> class.
    /// </summary>
    public RemoteServiceCallFailedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RemoteServiceCallFailedException" /> class with a specified error
    ///     message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RemoteServiceCallFailedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RemoteServiceCallFailedException" /> class with a specified error
    ///     message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RemoteServiceCallFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when an HTTP 403 Forbidden error occurs.
/// </summary>
/// <remarks>
///     This exception indicates that the user is authenticated but does not have permission to access the requested
///     resource.
/// </remarks>
public class ForbiddenAccessException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ForbiddenAccessException" /> class.
    /// </summary>
    public ForbiddenAccessException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ForbiddenAccessException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ForbiddenAccessException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ForbiddenAccessException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ForbiddenAccessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when attempting to create a profile with a username that already exists.
/// </summary>
public class ProfileUserNameAlreadyExistsException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfileUserNameAlreadyExistsException" /> class.
    /// </summary>
    public ProfileUserNameAlreadyExistsException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfileUserNameAlreadyExistsException" /> class with a specified error
    ///     message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ProfileUserNameAlreadyExistsException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfileUserNameAlreadyExistsException" /> class with a specified error
    ///     message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ProfileUserNameAlreadyExistsException(string? message, Exception? innerException) : base(message,
        innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when attempting to create a profile with a phone number that already exists.
/// </summary>
public class ProfilePhoneNumberAlreadyExistsException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfilePhoneNumberAlreadyExistsException" /> class.
    /// </summary>
    public ProfilePhoneNumberAlreadyExistsException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfilePhoneNumberAlreadyExistsException" /> class with a specified
    ///     error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ProfilePhoneNumberAlreadyExistsException(string? message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProfilePhoneNumberAlreadyExistsException" /> class with a specified
    ///     error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ProfilePhoneNumberAlreadyExistsException(string? message, Exception? innerException) : base(message,
        innerException)
    {
    }
}

/// <summary>
///     Represents an exception that is thrown when an HTTP 400 Bad Request error occurs.
/// </summary>
/// <remarks>
///     This exception indicates that the request was malformed or contains invalid data.
/// </remarks>
public class BadHttpRequestException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BadHttpRequestException" /> class.
    /// </summary>
    public BadHttpRequestException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BadHttpRequestException" /> class with a specified error message.
    /// </summary>
    /// <param name="titleAndContentAreRequired">The message that describes the error.</param>
    public BadHttpRequestException(string titleAndContentAreRequired) : base(titleAndContentAreRequired)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BadHttpRequestException" /> class with a specified error message and
    ///     inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BadHttpRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
