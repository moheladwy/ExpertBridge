using System.Text.Json;
using System.Text.Json.Nodes;
using ExpertBridge.GroqLibrary.Models;
using ExpertBridge.GroqLibrary.Settings;

namespace ExpertBridge.GroqLibrary.Clients;

/// <summary>
///     Provides methods for interacting with the Groq Vision APIs, enabling functionalities such as vision-based
///     completions
///     and tool-based integrations. This client is designed to facilitate communication with Groq's Vision API endpoints.
/// </summary>
/// <remarks>
///     The GroqApiVisionClient supports the creation of vision-based completions using HTTP-based requests, with the
///     ability
///     to handle inputs such as images (via URLs or Base64 encoding) and prompts. The client also provides functionality
///     to
///     integrate various tools to enrich the vision-based tasks. A reusable or dedicated instance of HttpClient can be
///     used
///     for making the requests. Additionally, users can specify models and configurations for controlling behavior.
/// </remarks>
public sealed class GroqApiVisionClient
{
    /// <summary>
    ///     The client responsible for interacting with chat completion functionalities in the Groq API.
    /// </summary>
    private readonly GroqApiChatCompletionClient _chatCompletionClient;

    /// <summary>
    ///     Provides methods for interacting with the Groq Vision APIs, leveraging HTTP-based requests for communication.
    /// </summary>
    /// <remarks>
    ///     This client is designed to integrate with the Groq API services for vision-specific functionalities and can
    ///     use a shared or dedicated HttpClient.
    ///     It also supports integration with the GroqApiChatCompletionClient for combined capabilities.
    /// </remarks>
    /// <param name="groqApiChatCompletionClient">
    ///     The client of type <see cref="GroqApiChatCompletionClient"/> responsible for handling chat completions
    /// </param>
    /// <param name="httpClient">
    ///     The <see cref="HttpClient"/> instance used for HTTP communication with the Groq API.
    /// </param>
    public GroqApiVisionClient(GroqApiChatCompletionClient groqApiChatCompletionClient, HttpClient httpClient)
    {
        _chatCompletionClient = groqApiChatCompletionClient;
    }

    /// <summary>
    ///     Base method for creating vision-based completions using the Groq API.
    /// </summary>
    /// <param name="request">The request object containing vision completion parameters.</param>
    /// <returns>The API response as a JsonObject.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid vision model is specified.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    private async Task<JsonObject?> CreateVisionCompletionAsync(JsonObject request)
    {
        ValidateVisionModel(request);
        return await _chatCompletionClient.CreateChatCompletionAsync(request);
    }

    /// <summary>
    ///     Creates a vision completion using an image URL with the Groq API.
    /// </summary>
    /// <param name="imageUrl">The URL of the image to analyze.</param>
    /// <param name="prompt">The text prompt describing the task or question about the image.</param>
    /// <param name="model">The vision model to use (default is "llama-3.2-90b-vision-preview").</param>
    /// <param name="temperature">Optional temperature setting to control randomness in the response.</param>
    /// <returns>The API response as a JsonObject containing the vision completion.</returns>
    /// <exception cref="ArgumentException">Thrown when the image URL is invalid.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async Task<JsonObject?> CreateVisionCompletionWithImageUrlAsync(
        string imageUrl,
        string prompt,
        string model = GroqApiVisionSettings.DefaultVisionModel,
        float? temperature = null)
    {
        ValidateImageUrl(imageUrl);

        var request = new JsonObject
        {
            ["model"] = model,
            ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = LlmRoles.UserRole,
                    ["content"] = new JsonArray
                    {
                        new JsonObject { ["type"] = "text", ["text"] = prompt },
                        new JsonObject
                        {
                            ["type"] = "image_url",
                            ["image_url"] = new JsonObject { ["url"] = imageUrl }
                        }
                    }
                }
            }
        };

        if (temperature.HasValue)
        {
            request["temperature"] = temperature.Value;
        }

        return await CreateVisionCompletionAsync(request);
    }

    /// <summary>
    ///     Creates a vision completion using a base64-encoded image with the Groq API.
    /// </summary>
    /// <param name="imagePath">The file path to the image to analyze.</param>
    /// <param name="prompt">The text prompt describing the task or question about the image.</param>
    /// <param name="model">The vision model to use (default is "llama-3.2-90b-vision-preview").</param>
    /// <param name="temperature">Optional temperature setting to control randomness in the response.</param>
    /// <returns>The API response as a JsonObject containing the vision completion.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the image file is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when the base64 encoded image exceeds size limits.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async Task<JsonObject?> CreateVisionCompletionWithBase64ImageAsync(
        string imagePath,
        string prompt,
        string model = GroqApiVisionSettings.DefaultVisionModel,
        float? temperature = null)
    {
        var base64Image = await ConvertImageToBase64(imagePath);
        ValidateBase64Size(base64Image);

        var request = new JsonObject
        {
            ["model"] = model,
            ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = new JsonArray
                    {
                        new JsonObject { ["type"] = "text", ["text"] = prompt },
                        new JsonObject
                        {
                            ["type"] = "image_url",
                            ["image_url"] = new JsonObject
                            {
                                ["url"] = $"data:image/jpeg;base64,{base64Image}"
                            }
                        }
                    }
                }
            }
        };

        if (temperature.HasValue)
        {
            request["temperature"] = temperature.Value;
        }

        return await CreateVisionCompletionAsync(request);
    }

    /// <summary>
    ///     Creates a vision completion with tool calling capabilities using the Groq API.
    /// </summary>
    /// <param name="imageUrl">The URL of the image to analyze.</param>
    /// <param name="prompt">The text prompt describing the task or question about the image.</param>
    /// <param name="tools">List of tools that the model can use to complete the task.</param>
    /// <param name="model">The vision model to use (default is llama-3.2-90b-vision-preview).</param>
    /// <returns>The API response as a JsonObject containing the vision completion with tool usage.</returns>
    /// <exception cref="ArgumentException">Thrown when the image URL is invalid.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async Task<JsonObject?> CreateVisionCompletionWithToolsAsync(
        string imageUrl,
        string prompt,
        List<Tool> tools,
        string model = GroqApiVisionSettings.DefaultVisionModel)
    {
        ValidateImageUrl(imageUrl);

        var request = new JsonObject
        {
            ["model"] = model,
            ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] =
                        new JsonArray
                        {
                            new JsonObject { ["type"] = "text", ["text"] = prompt },
                            new JsonObject
                            {
                                ["type"] = "image_url", ["image_url"] = new JsonObject { ["url"] = imageUrl }
                            }
                        }
                }
            },
            ["tools"] = JsonSerializer.SerializeToNode(tools.Select(t => new
            {
                type = t.Type,
                function = new
                {
                    name = t.Function.Name,
                    description = t.Function.Description,
                    parameters = t.Function.Parameters
                }
            })),
            ["tool_choice"] = "auto"
        };

        return await CreateVisionCompletionAsync(request);
    }

    /// <summary>
    ///     Creates a vision completion with JSON output format using the Groq API.
    /// </summary>
    /// <param name="imageUrl">The URL of the image to analyze.</param>
    /// <param name="prompt">The text prompt describing the task or question about the image.</param>
    /// <param name="model">The vision model to use (default is llama-3.2-90b-vision-preview).</param>
    /// <returns>The API response as a JsonObject containing the vision completion in JSON format.</returns>
    /// <exception cref="ArgumentException">Thrown when the image URL is invalid.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async Task<JsonObject?> CreateVisionCompletionWithJsonModeAsync(
        string imageUrl,
        string prompt,
        string model = GroqApiVisionSettings.DefaultVisionModel)
    {
        ValidateImageUrl(imageUrl);

        var request = new JsonObject
        {
            ["model"] = model,
            ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = new JsonArray
                    {
                        new JsonObject { ["type"] = "text", ["text"] = prompt },
                        new JsonObject
                        {
                            ["type"] = "image_url",
                            ["image_url"] = new JsonObject { ["url"] = imageUrl }
                        }
                    }
                }
            },
            ["response_format"] = new JsonObject { ["type"] = "json_object" }
        };

        return await CreateVisionCompletionAsync(request);
    }

    /// <summary>
    ///     Validates that the model specified in the request is a supported vision model.
    /// </summary>
    /// <param name="request">The request object containing the model specification.</param>
    /// <exception cref="ArgumentException">Thrown when an invalid vision model is specified.</exception>
    private static void ValidateVisionModel(JsonObject request)
    {
        var model = request["model"]?.GetValue<string>();
        if (string.IsNullOrEmpty(model) || !GroqApiVisionSettings.VisionModels.Contains(model))
        {
            throw new ArgumentException($"Invalid vision model. Must be one of: {GroqApiVisionSettings.VisionModels}");
        }
    }

    /// <summary>
    ///     Validates that the base64 encoded image string does not exceed the maximum allowed size.
    /// </summary>
    /// <param name="base64String">
    ///     The base64 encoded image string to validate
    /// </param>
    /// <exception cref="ArgumentException">Thrown when the base64 string exceeds the maximum allowed size.</exception>
    private static void ValidateBase64Size(string base64String)
    {
        var sizeInMb = base64String.Length * 3.0 / 4.0 / (1024 * 1024);
        if (sizeInMb > GroqApiVisionSettings.MaxBase64SizeMb)
        {
            throw new ArgumentException(
                $"Base64 encoded image exceeds maximum size of {GroqApiVisionSettings.MaxBase64SizeMb}MB");
        }
    }

    /// <summary>
    ///     Validates that the provided image URL is properly formatted.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the URL is invalid or empty.</exception>
    private static void ValidateImageUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException("Image URL cannot be null or empty");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            throw new ArgumentException("Invalid image URL format");
        }
    }

    /// <summary>
    ///     Converts an image file to a base64 encoded string.
    /// </summary>
    /// <param name="imagePath">The path to the image file.</param>
    /// <returns>A base64 encoded string representation of the image.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the image file is not found.</exception>
    private static async Task<string> ConvertImageToBase64(string imagePath)
    {
        if (!File.Exists(imagePath))
        {
            throw new FileNotFoundException($"Image file not found: {imagePath}");
        }

        var imageBytes = await File.ReadAllBytesAsync(imagePath);
        return Convert.ToBase64String(imageBytes);
    }
}
