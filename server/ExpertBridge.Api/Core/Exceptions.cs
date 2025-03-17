namespace ExpertBridge.Api.Core;

public class UserNotFoundException(string message) : Exception(message);

public class ProfileNotFoundException(string message) : Exception(message);

public class PostNotFoundException(string message) : Exception(message);

public class MediaNotFoundException(string message) : Exception(message);

public class CommentNotFoundException(string message) : Exception(message);
