namespace ExpertBridge.Api.Settings;

public class FirebaseSettings
{
    public const string Section = "Firebase";

    public string Type { get; set; }
    public string ProjectId { get; set; }
    public string PrivateKeyId { get; set; }
    public string PrivateKey { get; set; }
    public string ClientEmail { get; set; }
    public string ClientId { get; set; }
    public string AuthUri { get; set; }
    public string TokenUri { get; set; }
    public string AuthProviderX509CertUrl { get; set; }
    public string ClientX509CertUrl { get; set; }
    public string UniverseDomain { get; set; }
    public string AuthenticationTokenUri { get; set; }

}
