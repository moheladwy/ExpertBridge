namespace GroqLibrary.Settings;

/// <summary>
///     Contains settings for the Groq API vision features.
/// </summary>
public static class GroqApiVisionSettings
{
    /// <summary>Comma-separated list of supported vision model names.</summary>
    public const string VisionModels = "llama-3.2-90b-vision-preview,llama-3.2-11b-vision-preview";

    /// <summary>Default vision model name.</summary>
    public const string DefaultVisionModel = "llama-3.2-90b-vision-preview";

    /// <summary>Maximum allowed image size in megabytes.</summary>
    public const int MaxImageSizeMb = 20;

    /// <summary>Maximum allowed base64 encoded image size in megabytes.</summary>
    public const int MaxBase64SizeMb = 4;
}
