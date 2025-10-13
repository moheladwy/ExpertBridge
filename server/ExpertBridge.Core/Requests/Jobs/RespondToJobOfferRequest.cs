// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace ExpertBridge.Core.Requests.Jobs;

public class RespondToJobOfferRequest
{
    [Required] public bool Accept { get; set; }
}
