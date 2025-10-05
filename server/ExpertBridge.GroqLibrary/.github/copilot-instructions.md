# ExpertBridge.GroqLibrary - GitHub Copilot Instructions

## Project Purpose

Groq AI/LLM integration library providing clients for chat completion, vision, audio, and tool calling capabilities. Wraps the Groq API with strongly-typed .NET clients.

## Architecture Role

**External AI Service Integration** - Provides abstraction layer for Groq API interactions, enabling AI-powered features like content tagging, chat completions, and vision analysis.

## Key Responsibilities

-   Groq API client implementations
-   Chat completion with streaming support
-   Vision API for image analysis
-   Audio API for transcription
-   Tool calling for function execution
-   Request/response model definitions
-   API endpoint management

## Project Dependencies

```xml
Dependencies:
- None (pure library)

External:
- System.Text.Json
- System.Net.Http
```

## Client Pattern

### Chat Completion Client

```csharp
// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ExpertBridge.GroqLibrary.Settings;

namespace ExpertBridge.GroqLibrary.Clients;

/// <summary>
///     A client for interacting with the Groq API's chat completion functionality.
/// </summary>
/// <remarks>
///     This class provides methods to execute chat completion requests and receive responses.
///     It includes both synchronous and streaming support for responses. The client uses an
///     HttpClient instance for communicating with the Groq API, with authentication handled
///     via an API key provided during initialization. Ensure any HttpClient provided is properly
///     configured for network communication with the Groq API.
/// </remarks>
public sealed class GroqApiChatCompletionClient
{
    /// <summary>The HTTP client used for making API requests.</summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    ///     Initializes a new instance of the GroqApiChatCompletionClient with a provided HttpClient.
    /// </summary>
    /// <param name="httpClient">The HttpClient instance to use for API requests.</param>
    public GroqApiChatCompletionClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    ///     Creates a chat completion using the Groq API.
    /// </summary>
    /// <param name="request">The request object containing chat completion parameters.</param>
    /// <returns>The API response as a JsonObject.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async Task<JsonObject?> CreateChatCompletionAsync(JsonObject request)
    {
        var response = await _httpClient.PostAsJsonAsync(GroqApiEndpoints.ChatCompletionsEndpoint, request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"API request failed with status code {response.StatusCode}. Response content: {errorContent}");
        }

        return await response.Content.ReadFromJsonAsync<JsonObject>();
    }

    /// <summary>
    ///     Creates a streaming chat completion using the Groq API.
    /// </summary>
    /// <param name="request">The request object containing chat completion parameters.</param>
    /// <returns>An async enumerable of response chunks as JsonObjects.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async IAsyncEnumerable<JsonObject?> CreateChatCompletionStreamAsync(JsonObject request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        request["stream"] = true;

        var content = new StringContent(request.ToJsonString(), Encoding.UTF8, "application/json");
        using var requestMessage =
            new HttpRequestMessage(HttpMethod.Post, GroqApiEndpoints.ChatCompletionsEndpoint)
            {
                Content = content
            };

        using var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (line.StartsWith("data: "))
            {
                var data = line["data: ".Length..];
                if (data != "[DONE]")
                {
                    yield return JsonSerializer.Deserialize<JsonObject>(data);
                }
            }
        }
    }
}
```

## Settings Pattern

### API Endpoints Configuration

```csharp
namespace ExpertBridge.GroqLibrary.Settings;

/// <summary>
///     Contains the API endpoints for the Groq API.
/// </summary>
public static class GroqApiEndpoints
{
    public const string BaseUrl = "https://api.groq.com/openai/v1";
    public const string ChatCompletionsEndpoint = "chat/completions";
    public const string AudioTranscriptionsEndpoint = "audio/transcriptions";
    public const string AudioTranslationsEndpoint = "audio/translations";
}
```

### Settings Class

```csharp
namespace ExpertBridge.GroqLibrary.Settings;

public sealed class GroqSettings
{
    public const string SectionName = "Groq";

    public string ApiKey { get; set; }
    public string BaseUrl { get; set; } = GroqApiEndpoints.BaseUrl;
    public string Model { get; set; } = "llama-3.1-70b-versatile";
}
```

## Model Definitions

### Chat Message Models

```csharp
namespace ExpertBridge.GroqLibrary.Models;

public class ChatMessage
{
    public string Role { get; set; }
    public string Content { get; set; }
}

public class ChatCompletionRequest
{
    public string Model { get; set; }
    public List<ChatMessage> Messages { get; set; }
    public double? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public bool Stream { get; set; }
}

public class ChatCompletionResponse
{
    public string Id { get; set; }
    public string Object { get; set; }
    public long Created { get; set; }
    public string Model { get; set; }
    public List<Choice> Choices { get; set; }
    public Usage Usage { get; set; }
}

public class Choice
{
    public int Index { get; set; }
    public ChatMessage Message { get; set; }
    public string FinishReason { get; set; }
}

public class Usage
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}
```

## Provider Pattern

### HttpClient Factory Registration

```csharp
namespace ExpertBridge.GroqLibrary.Providers;

public static class GroqServiceProvider
{
    /// <summary>
    ///     Registers Groq API clients with the dependency injection container.
    /// </summary>
    public static IServiceCollection AddGroqServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var groqSettings = configuration.GetSection(GroqSettings.SectionName).Get<GroqSettings>()!;

        services.AddHttpClient<GroqApiChatCompletionClient>(client =>
        {
            client.BaseAddress = new Uri(groqSettings.BaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {groqSettings.ApiKey}");
        });

        services.AddHttpClient<GroqApiVisionClient>(client =>
        {
            client.BaseAddress = new Uri(groqSettings.BaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {groqSettings.ApiKey}");
        });

        services.AddHttpClient<GroqApiAudioClient>(client =>
        {
            client.BaseAddress = new Uri(groqSettings.BaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {groqSettings.ApiKey}");
        });

        services.AddHttpClient<GroqApiToolClient>(client =>
        {
            client.BaseAddress = new Uri(groqSettings.BaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {groqSettings.ApiKey}");
        });

        return services;
    }
}
```

## Usage Examples

### Basic Chat Completion

```csharp
public class MyService
{
    private readonly GroqApiChatCompletionClient _groqClient;

    public MyService(GroqApiChatCompletionClient groqClient)
    {
        _groqClient = groqClient;
    }

    public async Task<string> GetCompletionAsync(string prompt)
    {
        var request = new JsonObject
        {
            ["model"] = "llama-3.1-70b-versatile",
            ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = prompt
                }
            },
            ["temperature"] = 0.7,
            ["max_tokens"] = 1000
        };

        var response = await _groqClient.CreateChatCompletionAsync(request);
        return response?["choices"]?[0]?["message"]?["content"]?.ToString() ?? string.Empty;
    }
}
```

### Streaming Chat Completion

```csharp
public async Task StreamCompletionAsync(string prompt)
{
    var request = new JsonObject
    {
        ["model"] = "llama-3.1-70b-versatile",
        ["messages"] = new JsonArray
        {
            new JsonObject
            {
                ["role"] = "user",
                ["content"] = prompt
            }
        },
        ["stream"] = true
    };

    await foreach (var chunk in _groqClient.CreateChatCompletionStreamAsync(request))
    {
        var content = chunk?["choices"]?[0]?["delta"]?["content"]?.ToString();
        if (!string.IsNullOrEmpty(content))
        {
            Console.Write(content);
        }
    }
}
```

### With Structured Output (JSON Mode)

```csharp
public async Task<List<string>> ExtractTagsAsync(string content)
{
    var request = new JsonObject
    {
        ["model"] = "llama-3.1-70b-versatile",
        ["messages"] = new JsonArray
        {
            new JsonObject
            {
                ["role"] = "system",
                ["content"] = "You are a tagging expert. Extract relevant tags from the content as a JSON array."
            },
            new JsonObject
            {
                ["role"] = "user",
                ["content"] = content
            }
        },
        ["response_format"] = new JsonObject { ["type"] = "json_object" }
    };

    var response = await _groqClient.CreateChatCompletionAsync(request);
    var tagsJson = response?["choices"]?[0]?["message"]?["content"]?.ToString();

    return JsonSerializer.Deserialize<List<string>>(tagsJson ?? "[]") ?? [];
}
```

## Best Practices

1. **Use sealed classes** - All client classes should be sealed
2. **Use HttpClient properly** - Always inject via HttpClientFactory
3. **Handle API errors** - Check status codes and provide meaningful error messages
4. **Use JsonObject** - Leverage System.Text.Json.Nodes for flexible JSON handling
5. **Stream when appropriate** - Use streaming for long responses to improve UX
6. **Validate input** - Use ArgumentNullException.ThrowIfNull for parameters
7. **Document thoroughly** - Use XML documentation for all public APIs
8. **Configure timeouts** - Set appropriate HttpClient timeouts for API calls
9. **Log API calls** - Track token usage and API performance
10. **Handle rate limits** - Implement retry logic with exponential backoff

## Error Handling

```csharp
public async Task<JsonObject?> CreateChatCompletionAsync(JsonObject request)
{
    try
    {
        var response = await _httpClient.PostAsJsonAsync(GroqApiEndpoints.ChatCompletionsEndpoint, request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            _logger.LogError(
                "Groq API request failed with status {StatusCode}. Error: {Error}",
                response.StatusCode,
                errorContent);

            throw new HttpRequestException(
                $"API request failed with status code {response.StatusCode}. Response content: {errorContent}");
        }

        return await response.Content.ReadFromJsonAsync<JsonObject>();
    }
    catch (TaskCanceledException ex)
    {
        _logger.LogError(ex, "Groq API request timed out");
        throw new TimeoutException("The Groq API request timed out", ex);
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "Groq API request failed");
        throw;
    }
}
```

## Anti-Patterns to Avoid

-   ❌ Don't create HttpClient instances manually (use HttpClientFactory)
-   ❌ Don't hardcode API keys in code (use configuration)
-   ❌ Don't forget to dispose of streams in streaming scenarios
-   ❌ Don't ignore HTTP status codes
-   ❌ Don't use synchronous methods for I/O operations
-   ❌ Don't forget to handle cancellation tokens
-   ❌ Don't expose raw API responses (wrap in domain models)
-   ❌ Don't skip logging API calls (important for debugging and monitoring)
-   ❌ Don't forget to validate request parameters
-   ❌ Don't use string interpolation for JSON (use JsonObject)

## Configuration Example

### appsettings.json

```json
{
    "Groq": {
        "ApiKey": "your-groq-api-key-here",
        "BaseUrl": "https://api.groq.com/openai/v1",
        "Model": "llama-3.1-70b-versatile"
    }
}
```

### Program.cs Registration

```csharp
builder.Services.Configure<GroqSettings>(
    builder.Configuration.GetSection(GroqSettings.SectionName));

builder.Services.AddGroqServices(builder.Configuration);
```

## Folder Organization

```
ExpertBridge.GroqLibrary/
├── Clients/
│   ├── GroqApiChatCompletionClient.cs
│   ├── GroqApiVisionClient.cs
│   ├── GroqApiAudioClient.cs
│   └── GroqApiToolClient.cs
├── Models/
│   ├── ChatMessage.cs
│   ├── ChatCompletionRequest.cs
│   ├── ChatCompletionResponse.cs
│   └── ...
├── Interfaces/
│   └── IGroqClient.cs
├── Providers/
│   └── GroqServiceProvider.cs
└── Settings/
    ├── GroqSettings.cs
    └── GroqApiEndpoints.cs
```
