// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Extensions.CORS;

/// <summary>
/// Defines constant names for CORS (Cross-Origin Resource Sharing) policies used throughout the ExpertBridge application.
/// Provides centralized policy name references for API endpoints and SignalR hubs.
/// </summary>
public static class CorsPolicyNames
{
    /// <summary>
    /// CORS policy that allows all origins, methods, and headers for development and unrestricted API access.
    /// Should be used cautiously in production environments.
    /// </summary>
    public const string AllowAll = "AllowAll";

    /// <summary>
    /// CORS policy specifically configured for SignalR client connections with allowed origins and credentials support.
    /// Includes localhost development URLs and production Netlify deployment for real-time notification and chat features.
    /// </summary>
    public const string SignalRClients = "SignalRClients";
}
