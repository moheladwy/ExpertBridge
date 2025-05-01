using System.Text.Json;
using System.Text.Json.Nodes;
using ExpertBridge.GroqLibrary.Clients;
using ExpertBridge.GroqLibrary.Interfaces;
using ExpertBridge.GroqLibrary.Settings;

namespace ExpertBridge.GroqLibrary.Providers;

/// <summary>
///     Provides integration with Groq LLM (Large Language Model) API for text generation.
///     Implements <see cref="ILlmProvider" /> interface for consistent LLM operations
///     and <see cref="IDisposable" /> for proper resource cleanup.
/// </summary>
public class GroqLlmTextProvider : ILlmProvider, IDisposable
{
    private readonly GroqApiChatCompletionClient _client;
    private readonly string _model;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GroqLlmTextProvider" /> class.
    /// </summary>
    /// <param name="apiKey">The API key for authentication with Groq API.</param>
    /// <param name="model">The name of the LLM model to use for generation.</param>
    public GroqLlmTextProvider(string apiKey, string model)
    {
        _client = new GroqApiChatCompletionClient(apiKey);
        _model = model;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GroqLlmTextProvider" /> class with a custom HTTP client.
    /// </summary>
    /// <param name="apiKey">The API key for authentication with Groq API.</param>
    /// <param name="model">The name of the LLM model to use for generation.</param>
    /// <param name="httpClient">The HTTP client to use for API requests.</param>
    public GroqLlmTextProvider(string apiKey, string model, HttpClient httpClient)
    {
        _client = new GroqApiChatCompletionClient(apiKey, httpClient);
        _model = model;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose() => _client.Dispose();

    /// <summary>
    ///     Generates text based on the provided user prompt using the configured LLM model.
    /// </summary>
    /// <param name="userPrompt">The user's input prompt for text generation.</param>
    /// <returns>The generated text response from the model.</returns>
    public async Task<string> GenerateAsync(string userPrompt)
    {
        var request = new JsonObject
        {
            ["model"] = _model,
            ["messages"] = JsonSerializer.SerializeToNode(new[]
            {
                new { role = LlmRoles.UserRole, content = userPrompt }
            })
        };

        var response = await _client.CreateChatCompletionAsync(request);
        return response?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? string.Empty;
    }

    /// <summary>
    ///     Generates text based on both system and user prompts using the configured LLM model.
    /// </summary>
    /// <param name="systemPrompt">The system instructions or context for the generation.</param>
    /// <param name="userPrompt">The user's input prompt for text generation.</param>
    /// <returns>The generated text response from the model.</returns>
    public async Task<string> GenerateAsync(string systemPrompt, string userPrompt)
    {
        var request = new JsonObject
        {
            ["model"] = _model,
            ["messages"] = JsonSerializer.SerializeToNode(new[]
            {
                new { role = LlmRoles.SystemRole, content = systemPrompt },
                new { role = LlmRoles.UserRole, content = userPrompt }
            })
        };

        var response = await _client.CreateChatCompletionAsync(request);
        return response?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? string.Empty;
    }
}
