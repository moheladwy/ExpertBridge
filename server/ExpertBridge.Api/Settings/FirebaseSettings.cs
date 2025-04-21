<<<<<<< HEAD
namespace ExpertBridge.Api.Settings;

public class FirebaseSettings
{
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
=======
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace ExpertBridge.Api.Settings;

internal class FirebaseSettings
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("project_id")]
    public string ProjectId { get; set; }

    [JsonPropertyName("private_key_id")]
    public string PrivateKeyId { get; set; }

    [JsonPropertyName("private_key")]
    public string PrivateKey { get; set; }

    [JsonPropertyName("client_email")]
    public string ClientEmail { get; set; }

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }

    [JsonPropertyName("auth_uri")]
    public string AuthUri { get; set; }

    [JsonPropertyName("token_uri")]
    public string TokenUri { get; set; }

    [JsonPropertyName("auth_provider_x509_cert_url")]
    public string AuthProviderX509CertUrl { get; set; }

    [JsonPropertyName("client_x509_cert_url")]
    public string ClientX509CertUrl { get; set; }

    [JsonPropertyName("universe_domain")]
    public string UniverseDomain { get; set; }
>>>>>>> main
}
