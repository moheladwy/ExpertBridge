namespace ExpertBridge.Application.Settings;

/// <summary>
///     Represents Firebase service account credentials and configuration for the ExpertBridge application.
///     Contains OAuth 2.0 credentials and project details for Firebase Admin SDK integration.
/// </summary>
/// <remarks>
///     These settings are loaded from the "Firebase" configuration section and typically sourced from
///     a Firebase service account JSON file (FirebaseOAuthCredentialsExpertBridge.json).
///     Used to configure:
///     **Configured in appsettings.json under "Firebase" section:**
///     <code>
///         {
///           "Firebase": {
///             "Type": "service_account",
///             "ProjectId": "your-project-id",
///             "PrivateKeyId": "key-id-from-json",
///             "PrivateKey": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n",
///             "ClientEmail": "firebase-adminsdk-xxxxx@your-project-id.iam.gserviceaccount.com",
///             "ClientId": "1234567890",
///             "AuthUri": "https://accounts.google.com/o/oauth2/auth",
///             "TokenUri": "https://oauth2.googleapis.com/token",
///             "AuthProviderX509CertUrl": "https://www.googleapis.com/oauth2/v1/certs",
///             "ClientX509CertUrl": "https://www.googleapis.com/robot/v1/metadata/x509/...",
///             "UniverseDomain": "googleapis.com",
///             "AuthenticationTokenUri": "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword"
///           }
///         }
///     </code>
///     - Firebase Admin SDK for server-side operations
///     - Firebase Authentication for JWT token validation
///     - Firebase Cloud Messaging for push notifications
///     - HTTP client for Firebase REST API calls
///     The service account provides administrative access to Firebase services including user management,
///     authentication token verification, and cloud messaging.
/// </remarks>
public sealed class FirebaseSettings
{
    /// <summary>
    ///     Gets the configuration section name for Firebase settings.
    /// </summary>
    public const string Section = "Firebase";

    /// <summary>
    ///     Gets or sets the type of the Firebase service account (typically "service_account").
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    ///     Gets or sets the Firebase project identifier for the ExpertBridge application.
    /// </summary>
    public string ProjectId { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier for the service account's private key.
    /// </summary>
    public string PrivateKeyId { get; set; }

    /// <summary>
    ///     Gets or sets the RSA private key in PEM format for service account authentication.
    /// </summary>
    public string PrivateKey { get; set; }

    /// <summary>
    ///     Gets or sets the email address of the Firebase service account.
    /// </summary>
    public string ClientEmail { get; set; }

    /// <summary>
    ///     Gets or sets the unique client identifier for the service account.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    ///     Gets or sets the OAuth 2.0 authorization server endpoint URL.
    /// </summary>
    public string AuthUri { get; set; }

    /// <summary>
    ///     Gets or sets the OAuth 2.0 token endpoint URL for obtaining access tokens.
    /// </summary>
    public string TokenUri { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the public x509 certificate for the OAuth 2.0 authorization server.
    /// </summary>
    public string AuthProviderX509CertUrl { get; set; }

    /// <summary>
    ///     Gets or sets the URL of the public x509 certificate for this service account.
    /// </summary>
    public string ClientX509CertUrl { get; set; }

    /// <summary>
    ///     Gets or sets the universe domain for the service account (typically "googleapis.com").
    /// </summary>
    public string UniverseDomain { get; set; }

    /// <summary>
    ///     Gets or sets the custom authentication token URI for Firebase REST API calls.
    /// </summary>
    public string AuthenticationTokenUri { get; set; }
}
