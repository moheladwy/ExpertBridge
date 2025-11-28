// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace ExpertBridge.Contract.Requests.RespondToJobOffer;

/// <summary>
///     Represents a request to respond to a job offer.
/// </summary>
/// <remarks>
///     Workers can accept or decline job offers extended to them by hirers.
/// </remarks>
public sealed class RespondToJobOfferRequest
{
    /// <summary>
    ///     Gets or sets a value indicating whether the job offer is accepted.
    /// </summary>
    /// <remarks>
    ///     Set to true to accept the offer, false to decline.
    /// </remarks>
    [Required]
    public bool Accept { get; set; }
}
