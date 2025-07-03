using System;

namespace ExpertBridge.Admin.ViewModels;

public class PostTagsViewModel
{
    public string TagId { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string ArabicName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PostCount { get; set; }
}
