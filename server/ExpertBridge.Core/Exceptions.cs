namespace ExpertBridge.Core;

public class UserNotFoundException(string message) : Exception(message);

public class ProfileNotFoundException(string message) : Exception(message);
