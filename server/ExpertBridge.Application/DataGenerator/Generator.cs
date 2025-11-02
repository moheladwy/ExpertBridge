using Pgvector;

namespace ExpertBridge.Application.DataGenerator;

/// <summary>
///     A static class responsible for generating random pgvector Vectors for testing and simulation.
/// </summary>
/// <remarks>
///     This class is typically used to generate random AI embeddings for simulation purposes without requiring actual
///     embedding models.
/// </remarks>
public static class Generator
{
    /// <summary>
    ///     Represents a private instance of the Random class used for generating random values.
    /// </summary>
    /// <remarks>
    ///     This Random instance is used internally for creating random vectors with floating-point values.
    ///     It is instantiated once and shared across the methods in the Generator class to ensure consistent and efficient
    ///     random number generation.
    /// </remarks>
    private static readonly Random random = new();

    /// <summary>
    ///     Generates a random pgvector Vector with specified dimensions.
    /// </summary>
    /// <param name="dimensions">The number of dimensions (typically 1024 for ExpertBridge).</param>
    /// <returns>A Pgvector Vector with random float values in range [-1, 1].</returns>
    /// <remarks>
    ///     Used for testing AI recommendation algorithms without running actual embedding models.
    ///     The default dimension is 1024 to match Ollama mxbai-embed-large model output.
    ///     **Warning: ** Random vectors to have NO semantic meaning and will produce random similarity results.
    /// </remarks>
    public static Vector GenerateRandomVector(int dimensions)
    {
        var values = new float[dimensions];
        for (var i = 0; i < dimensions; i++)
        {
            values[i] = (float)(random.NextDouble() * 2 - 1);
        }

        return new Vector(values);
    }
}
