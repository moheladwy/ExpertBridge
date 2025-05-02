using System.Net.Http.Headers;
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
public class GroqApiAudioClient : IDisposable
{
    /// <summary>
    ///     The client responsible for interacting with chat completion functionalities in the Groq API.
    /// </summary>
    private readonly GroqApiChatCompletionClient _chatCompletionClient;

    /// <summary>The HTTP client used for making API requests.</summary>
    private readonly HttpClient _httpClient;


    /// <summary>
    ///     Provides methods for interacting with the Groq Vision APIs, leveraging HTTP-based requests for communication.
    /// </summary>
    /// <remarks>
    ///     This client is designed to integrate with the Groq API services for vision-specific functionalities and can utilize
    ///     a shared or dedicated HttpClient.
    ///     It also supports integration with the GroqApiChatCompletionClient for combined capabilities.
    /// </remarks>
    /// <param name="apiKey">The API key for authentication with Groq services.</param>
    public GroqApiAudioClient(string apiKey)
    {
        _chatCompletionClient = new GroqApiChatCompletionClient(apiKey);
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    /// <summary>
    ///     Initializes a new instance of the GroqApiAudioClient with a provided HttpClient.
    /// </summary>
    /// <remarks>This constructor allows for the use of a shared HttpClient for API requests.</remarks>
    /// <param name="apiKey">The API key for authentication with Groq services.</param>
    /// <param name="httpClient">The HttpClient instance to use for API requests.</param>
    public GroqApiAudioClient(string apiKey, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _chatCompletionClient = new GroqApiChatCompletionClient(apiKey, httpClient);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    /// <summary>
    ///     Releases the resources used by the GroqApiAudioClient, including the associated HttpClient.
    /// </summary>
    public void Dispose()
    {
        _httpClient.Dispose();
        _chatCompletionClient.Dispose();
        GC.SuppressFinalize(this);
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
        content.Add(new StreamContent(audioFile), "file", fileName);
        content.Add(new StringContent(model), "model");

        if (!string.IsNullOrEmpty(prompt))
        {
            content.Add(new StringContent(prompt), "prompt");
        }

        content.Add(new StringContent(responseFormat), "response_format");

        if (!string.IsNullOrEmpty(language))
        {
            content.Add(new StringContent(language), "language");
        }

        if (temperature.HasValue)
        {
            content.Add(new StringContent(temperature.Value.ToString()), "temperature");
        }

        var response = await _httpClient.PostAsync(GroqApiEndpoints.BaseUrl + GroqApiEndpoints.TranscriptionsEndpoint,
            content);
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
        content.Add(new StreamContent(audioFile), "file", fileName);
        content.Add(new StringContent(model), "model");

        if (!string.IsNullOrEmpty(prompt))
        {
            content.Add(new StringContent(prompt), "prompt");
        }

        content.Add(new StringContent(responseFormat), "response_format");

        if (temperature.HasValue)
        {
            content.Add(new StringContent(temperature.Value.ToString()), "temperature");
        }

        var response =
            await _httpClient.PostAsync(GroqApiEndpoints.BaseUrl + GroqApiEndpoints.TranslationsEndpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<JsonObject>();
    }
}
