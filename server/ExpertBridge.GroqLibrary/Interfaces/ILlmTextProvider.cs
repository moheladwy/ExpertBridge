// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.GroqLibrary.Interfaces;

/// <summary>
///     Provides an interface for interacting with Large Language Models (LLM) to generate text responses.
/// </summary>
public interface ILlmTextProvider
{
    /// <summary>
    ///     Generates a response using the LLM based on the user's prompt.
    /// </summary>
    /// <param name="userPrompt">The user's input prompt for text generation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the generated text response.</returns>
    Task<string> GenerateAsync(string userPrompt);

    /// <summary>
    ///     Generates a response using the LLM based on both system and user prompts.
    /// </summary>
    /// <param name="systemPrompt">The system prompt providing context or instructions to the LLM.</param>
    /// <param name="userPrompt">The user's input prompt for text generation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the generated text response.</returns>
    Task<string> GenerateAsync(string systemPrompt, string userPrompt);
}
