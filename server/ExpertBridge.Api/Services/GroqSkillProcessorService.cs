using System.Text.Json;
using ExpertBridge.Api.Models.GroqResponses;
using ExpertBridge.Api.Settings;
using ExpertBridge.GroqLibrary.Providers;
using Polly;
using Polly.Registry;

namespace ExpertBridge.Api.Services;

/// <summary>
/// Service responsible for processing and analyzing skills using the Groq Large Language Model (LLM).
/// This service provides functionality to generate detailed descriptions for skills by leveraging
/// natural language processing capabilities.
/// </summary>
/// <remarks>
/// This service implements resilience patterns to handle potential failures in LLM responses
/// and JSON deserialization operations.
/// </remarks>
public sealed class GroqSkillProcessorService
{
    /// <summary>
    ///     An instance of <see cref="GroqLlmTextProvider" /> used to interact with the Groq Large Language Model (LLM)
    ///     API for generating text-based categorizations in the context of post-analysis.
    /// </summary>
    private readonly GroqLlmTextProvider _groqLlmTextProvider;

    /// <summary>
    ///     An instance of <see cref="JsonSerializerOptions" /> configured for deserializing JSON responses in a
    ///     case-insensitive manner,
    ///     ensuring robust parsing of post-categorization results from the GroqLlmTextProvider.
    /// </summary>
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// An instance of <see cref="ResiliencePipeline"/> used to handle retries and fault tolerance
    /// when processing malformed JSON responses from the Groq LLM API.
    /// </summary>
    private readonly ResiliencePipeline _resiliencePipeline;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroqSkillProcessorService"/> class.
    /// </summary>
    /// <param name="groqLlmTextProvider">The Groq LLM text provider for generating skill descriptions.</param>
    /// <param name="resilience">The resilience pipeline provider for handling failures and retries.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the required parameters is null.</exception>
    public GroqSkillProcessorService(
        GroqLlmTextProvider groqLlmTextProvider,
        ResiliencePipelineProvider<string> resilience)
    {
        _groqLlmTextProvider = groqLlmTextProvider;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowOutOfOrderMetadataProperties = true,
            AllowTrailingCommas = true,
        };
        _resiliencePipeline = resilience.GetPipeline(ResiliencePipelines.MalformedJsonModelResponse);
    }

    /// <summary>
    /// Processes a collection of skills and generates detailed descriptions for each skill using the Groq LLM.
    /// </summary>
    /// <param name="skills">A read-only collection of skill names to be processed.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>
    /// A <see cref="Task{ProcessedSkillsResponse}"/> representing the asynchronous operation.
    /// The task result contains the processed skills with their descriptions.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the skills collection is null or empty.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when JSON deserialization fails or when the LLM response cannot be processed.
    /// </exception>
    /// <remarks>
    /// This method uses a resilience pipeline to handle potential failures in LLM communication
    /// and JSON parsing operations. The method will retry failed operations according to the
    /// configured resilience policy.
    /// </remarks>
    public async Task<ProcessedSkillsResponse> ProcessSkillsAsync(
        IReadOnlyCollection<string> skills,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (skills == null || skills.Count == 0)
                throw new ArgumentException("Skills list cannot be null or empty.", nameof(skills));

            var result = await _resiliencePipeline.ExecuteAsync<ProcessedSkillsResponse>(async ct =>
            {
                var systemPrompt = GetSystemPrompt();
                var userPrompt = GetUserPrompt(skills);
                var response = await _groqLlmTextProvider.GenerateAsync(systemPrompt, userPrompt);
                var processedSkillsResponse =
                    JsonSerializer.Deserialize<ProcessedSkillsResponse>(response, _jsonSerializerOptions)
                    ?? throw new InvalidOperationException(
                        "Failed to deserialize the categorizer response: null result");
                return processedSkillsResponse;
            }, cancellationToken);

            return result;
        }
        catch (JsonException ex)
        {
            // Log the exception or handle it as needed
            throw new InvalidOperationException("An error occurred while processing skills.", ex);
        }
    }

    /// <summary>
    /// Reads and returns the JSON schema format for the expected output structure of processed skills.
    /// </summary>
    /// <returns>A string containing the JSON schema definition for the processed skills output format.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the output format file cannot be found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when access to the output format file is denied.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs while reading the output format file.</exception>
    /// <remarks>
    /// The schema file is expected to be located at "LlmOutputFormat/ProcessedSkillsOutputFormat.json"
    /// relative to the application's working directory.
    /// </remarks>
    private static string GetOutputFormatSchema() => File.ReadAllText("LlmOutputFormat/ProcessedSkillsOutputFormat.json");

    /// <summary>
    ///   Generates the system prompt for the Groq LLM to process skills.
    /// </summary>
    /// <returns>
    ///     A string containing the formatted system prompt with specific instructions for the processing skills.
    /// </returns>
    private static string GetSystemPrompt()
    {
        List<string> systemPrompt =
        [
            "You are an advanced skills descriptor AI Model, specializing in analyzing and processing skills.",
            "Your task is to process a list of skills and provide detailed descriptions for each skill.",
            "Provide a structured output with the following format:",
            "```json",
            GetOutputFormatSchema(),
            "```",
            "Do not generate any introductory or concluding text.",
            "Do not include any additional explanations or comments.",
            "Ensure that the output is strictly in JSON format without any additional text.",
            "The output should include the skill name and a detailed description of the skill.",
            "If a skill is not recognized, return an empty description for that skill.",
            "Do not change the skill names at all.",
            "Do not change the meaning of the skills or their descriptions.",
            "Do not include any tags or additional metadata in the output.",
        ];
        return string.Join("\n", systemPrompt);
    }

    /// <summary>
    /// Generates the user prompt that contains the skills to be processed by the Groq LLM.
    /// </summary>
    /// <param name="skills">A read-only collection of skill names to include in the prompt.</param>
    /// <returns>A formatted string containing the user prompt with the skills list in JSON format.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the skills parameter is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown when There is no compatible System.Text.Json.Serialization.JsonConverter for TValue or its serializable members.
    /// </exception>
    /// <remarks>
    /// The generated prompt includes the skills in JSON format and instructions for the LLM
    /// to provide detailed descriptions for each skill.
    /// </remarks>
    private static string GetUserPrompt(IReadOnlyCollection<string> skills)
    {
        List<string> userPrompt =
        [
            "Here is the list of skills to process:",
            "```json",
            JsonSerializer.Serialize(skills.ToList()),
            "```",
            "Please provide detailed descriptions for each skill in the specified format.",
        ];
        return string.Join("\n", userPrompt);
    }
}
