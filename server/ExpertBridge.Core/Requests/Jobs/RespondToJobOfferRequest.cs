using System;
using System.ComponentModel.DataAnnotations;

namespace ExpertBridge.Core.Requests.Jobs;

public class RespondToJobOfferRequest
{
    [Required]
    public bool Accept {get; set;}
    
}
