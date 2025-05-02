namespace ExpertBridge.GroqLibrary.Settings;

/// <summary>
///     Provides predefined role constants for use in API requests involving language models (LLMs).
/// </summary>
/// <remarks>
///     This class defines roles commonly used in interactions with language models:
///     - <c>SystemRole</c>, <c>UserRole</c>, <c>ToolRole</c>, <c>AssistantRole</c>
/// </remarks>
public static class LlmRoles
{
    /// <summary>
    ///     Represents the constant value associated with the system's role in a conversation with language models (LLMs).
    /// </summary>
    /// <remarks>
    ///     The <c>SystemRole</c> is typically used to specify the system's part in structuring the interaction.
    ///     It is commonly utilized to set initial instructions, context, or parameters for the conversation
    ///     when making API requests involving LLMs.
    /// </remarks>
    public const string SystemRole = "system";

    /// <summary>
    ///     Represents the constant value associated with the user's role in interactions with the language models.
    /// </summary>
    /// <remarks>
    ///     The <c>UserRole</c> is used to identify the user's contributions or inputs during conversational exchanges with the
    ///     language model.
    ///     This role typically signifies user-provided prompts, requests, or any input context during API interactions.
    /// </remarks>
    public const string UserRole = "user";

    /// <summary>
    ///     Represents the constant value associated with a tool's role in a conversation with language models (LLMs).
    /// </summary>
    /// <remarks>
    ///     The <c>ToolRole</c> is used to signify responses or outputs from tools or functions invoked
    ///     during an interaction with language models. This role is commonly utilized to categorize
    ///     messages or outputs that originate from external functions or systems during API workflows.
    /// </remarks>
    public const string ToolRole = "tool";

    /// <summary>
    /// Represents the constant value associated with the assistant's role in a conversation with language models (LLMs).
    /// </summary>
    /// <remarks>
    /// The <c>AssistantRole</c> is typically used to define the role of the language model acting as an assistant
    /// in the interaction. It helps in specifying the assistant's responses and behavior in such exchanges.
    /// </remarks>
    public const string AssistantRole = "assistant";
}
