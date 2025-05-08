namespace ExpertBridge.Extensions.Firebase;

public sealed class FirebaseAuthSettings
{
    public const string Section = "Authentication:Firebase";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string TokenUri { get; set; } = string.Empty;
}
