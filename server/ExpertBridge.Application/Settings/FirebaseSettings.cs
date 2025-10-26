namespace ExpertBridge.Application.Settings;

/// <summary>
///     Configuration settings for Firebase Admin SDK service account credentials.
/// </summary>
/// <remarks>
///     This settings class contains the complete Firebase service account JSON credentials
///     required for server-side Firebase operations (Admin SDK).
///     **Configured in appsettings.json under "Firebase" section:**
///     <code>
/// {
///   "Firebase": {
///     "Type": "service_account",
///     "ProjectId": "your-project-id",
///     "PrivateKeyId": "key-id-from-json",
///     "PrivateKey": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n",
///     "ClientEmail": "firebase-adminsdk-xxxxx@your-project-id.iam.gserviceaccount.com",
///     "ClientId": "1234567890",
///     "AuthUri": "https://accounts.google.com/o/oauth2/auth",
///     "TokenUri": "https://oauth2.googleapis.com/token",
///     "AuthProviderX509CertUrl": "https://www.googleapis.com/oauth2/v1/certs",
///     "ClientX509CertUrl": "https://www.googleapis.com/robot/v1/metadata/x509/...",
///     "UniverseDomain": "googleapis.com",
///     "AuthenticationTokenUri": "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword"
///   }
/// }
/// </code>
///     **Security Critical:**
///     - This file contains your Firebase private key and must NEVER be committed to source control
///     - Use User Secrets for development: `dotnet user-secrets set "Firebase:PrivateKey" "..."`
///     - Use Azure Key Vault or environment variables for production
///     - Rotate keys immediately if accidentally exposed
///     **Obtaining Credentials:**
///     1. Go to Firebase Console > Project Settings > Service Accounts
///     2. Click "Generate New Private Key"
///     3. Download the JSON file
///     4. Map JSON properties to this settings class
///     **Use Cases:**
///     - Server-side user management and authentication
///     - Custom token generation
///     - Firebase Cloud Messaging
///     - Firestore/Realtime Database access with admin privileges
///     The AuthenticationTokenUri property extends the standard service account JSON with
///     the Firebase Authentication REST API endpoint for password-based sign-in.
/// </remarks>
public sealed class FirebaseSettings
{
    /// <summary>
    ///     The configuration section name in appsettings.json.
    /// </summary>
    public const string Section = "Firebase";

    /// <summary>
    ///     Gets or sets the type of credential (always "service_account" for Firebase Admin SDK).
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    ///     Gets or sets the Firebase project identifier.
    /// </summary>
    public string ProjectId { get; set; }

    /// <summary>
    ///     Gets or sets the private key identifier from the service account JSON.
    /// </summary>
    public string PrivateKeyId { get; set; }

    /// <summary>
    ///     Gets or sets the RSA private key in PEM format for signing custom tokens and API requests.
    /// </summary>
    /// <remarks>
    ///     Format: "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n"
    ///     Keep this value secure - it grants full admin access to your Firebase project.
    /// </remarks>
    public string PrivateKey { get; set; }

    /// <summary>
    ///     Gets or sets the service account email address.
    /// </summary>
    /// <remarks>
    ///     Format: firebase-adminsdk-xxxxx@{project-id}.iam.gserviceaccount.com
    /// </remarks>
    public string ClientEmail { get; set; }

    /// <summary>
    ///     Gets or sets the service account client ID (numeric string).
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    ///     Gets or sets the OAuth2 authorization endpoint URL.
    /// </summary>
    public string AuthUri { get; set; }

    /// <summary>
    ///     Gets or sets the OAuth2 token endpoint URL for obtaining access tokens.
    /// </summary>
    public string TokenUri { get; set; }

    /// <summary>
    ///     Gets or sets the URL for Google's OAuth2 public certificates.
    /// </summary>
    public string AuthProviderX509CertUrl { get; set; }

    /// <summary>
    ///     Gets or sets the URL for this service account's X.509 public certificate.
    /// </summary>
    /// <remarks>
    ///     Used for verifying signatures created by this service account.
    /// </remarks>
    public string ClientX509CertUrl { get; set; }

    /// <summary>
    ///     Gets or sets the universe domain (typically "googleapis.com").
    /// </summary>
    public string UniverseDomain { get; set; }

    /// <summary>
    ///     Gets or sets the Firebase Authentication REST API endpoint for password-based sign-in.
    /// </summary>
    /// <remarks>
    ///     Custom property not in standard service account JSON.
    ///     Used for server-side user authentication with email/password.
    ///     Format: https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={API_KEY}
    /// </remarks>
    public string AuthenticationTokenUri { get; set; }
}
