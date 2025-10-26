// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Core.Messages;

/// <summary>
/// Represents a message to process user interest tags via RabbitMQ/MassTransit.
/// </summary>
/// <remarks>
/// This message triggers the generation of vector embeddings for user interests to enable
/// personalized recommendations and content matching using semantic similarity.
/// </remarks>
public class UserInterestsProsessingMessage
{
    /// <summary>
    /// Gets or sets the unique identifier of the user profile.
    /// </summary>
    public string UserProfileId { get; set; }

    /// <summary>
    /// Gets or sets the list of interest tags to process.
    /// </summary>
    public List<string> InterestsTags { get; set; }
}
