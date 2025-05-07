using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using ExpertBridge.GroqLibrary.Settings;

namespace ExpertBridge.GroqLibrary.Clients;

/// <summary>
///     Represents a client for interacting with the audio-related services of the Groq API.
/// </summary>
/// <remarks>
///     This client provides methods for performing audio-specific operations, such as transcriptions and translations,
///     by leveraging the Groq API. It can be initialized with its own HttpClient or use a shared one for network
///     communication. The class also includes mechanisms for integrating with the GroqApiChatCompletionClient.
/// </remarks>
public sealed class GroqApiAudioClient
{
    /// <summary>
    ///     The client responsible for interacting with chat completion functionalities in the Groq API.
    /// </summary>
    private readonly GroqApiChatCompletionClient _chatCompletionClient;

    /// <summary>The HTTP client used for making API requests.</summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    ///     Initializes a new instance of the GroqApiAudioClient with a provided HttpClient.
    /// </summary>
    /// <remarks>This constructor allows for the use of a shared HttpClient for API requests.</remarks>
    /// <param name="groqApiChatCompletionClient">
    ///     The <see cref="GroqApiChatCompletionClient"/> instance to be used for chat completion functionalities.
    /// </param>
    /// <param name="httpClient">
    ///     The <see cref="HttpClient"/> instance to use for audio-related API requests.
    /// </param>
    public GroqApiAudioClient(GroqApiChatCompletionClient groqApiChatCompletionClient, HttpClient httpClient)
    {
        _chatCompletionClient = groqApiChatCompletionClient;
        _httpClient = httpClient;
    }

    /// <summary>
    ///     Creates a transcription of an audio file using the Groq API.
    /// </summary>
    /// <param name="audioFile">The audio file stream to transcribe.</param>
    /// <param name="fileName">The name of the audio file.</param>
    /// <param name="model">The model to use for transcription.</param>
    /// <param name="prompt">Optional prompt to guide the transcription.</param>
    /// <param name="responseFormat">The format of the response (default is "json").</param>
    /// <param name="language">Optional language specification for the audio.</param>
    /// <param name="temperature">Optional temperature setting for the transcription.</param>
    /// <returns>The API response as a JsonObject containing the transcription.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async Task<JsonObject?> CreateTranscriptionAsync(Stream audioFile, string fileName, string model,
        string? prompt = null, string responseFormat = "json", string? language = null, float? temperature = null)
    {
        using var content = new MultipartFormDataContent();

        using var fileContent = new StreamContent(audioFile);
        content.Add(fileContent, "file", fileName);

        using var modelContent = new StringContent(model);
        content.Add(modelContent, "model");

        if (!string.IsNullOrEmpty(prompt))
        {
            using var promptContent = new StringContent(prompt);
            content.Add(promptContent, "prompt");
        }

        using var responseFormatContent = new StringContent(responseFormat);
        content.Add(responseFormatContent, "response_format");

        if (!string.IsNullOrEmpty(language))
        {
            using var languageContent = new StringContent(language);
            content.Add(languageContent, "language");
        }

        if (temperature.HasValue)
        {
            using var temperatureContent = new StringContent(temperature.Value.ToString(CultureInfo.CurrentCulture));
            content.Add(temperatureContent, "temperature");
        }

        var response = await _httpClient.PostAsync(GroqApiEndpoints.TranscriptionsEndpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<JsonObject>();
    }

    /// <summary>
    ///     Creates a translation of an audio file to English using the Groq API.
    /// </summary>
    /// <param name="audioFile">The audio file stream to translate.</param>
    /// <param name="fileName">The name of the audio file.</param>
    /// <param name="model">The model to use for translation.</param>
    /// <param name="prompt">Optional prompt to guide the translation.</param>
    /// <param name="responseFormat">The format of the response (default is "json").</param>
    /// <param name="temperature">Optional temperature setting for the translation.</param>
    /// <returns>The API response as a JsonObject containing the translation.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async Task<JsonObject?> CreateTranslationAsync(Stream audioFile, string fileName, string model,
        string? prompt = null, string responseFormat = "json", float? temperature = null)
    {
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(audioFile);
        content.Add(fileContent, "file", fileName);

        using var modelContent = new StringContent(model);
        content.Add(modelContent, "model");

        if (!string.IsNullOrEmpty(prompt))
        {
            using var promptContent = new StringContent(prompt);
            content.Add(promptContent, "prompt");
        }

        using var responseFormatContent = new StringContent(responseFormat);
        content.Add(responseFormatContent, "response_format");

        if (temperature.HasValue)
        {
            using var temperatureContent = new StringContent(temperature.Value.ToString(CultureInfo.CurrentCulture));
            content.Add(temperatureContent, "temperature");
        }

        var response = await _httpClient.PostAsync(GroqApiEndpoints.TranslationsEndpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<JsonObject>();
    }
}
