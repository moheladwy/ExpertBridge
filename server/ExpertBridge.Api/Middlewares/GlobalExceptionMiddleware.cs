using System.Diagnostics;

namespace ExpertBridge.Api.Middlewares;

/// <summary>
///     Middleware to handle global exceptions in the application.
/// </summary>
internal class GlobalExceptionMiddleware
{
    /// <summary>
    ///     The RequestDelegate instance to use for the next middleware in the pipeline.
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    ///     The ILogger instance to use for logging exceptions.
    /// </summary>
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    /// <summary>
    ///     Constructor for the GlobalExceptionMiddleware class.
    /// </summary>
    /// <param name="logger">
    ///     The ILogger instance to use for logging exceptions.
    /// </param>
    /// <param name="next">
    ///     The RequestDelegate instance to use for the next middleware in the pipeline.
    /// </param>
    public GlobalExceptionMiddleware(
        ILogger<GlobalExceptionMiddleware> logger,
        RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    /// <summary>
    ///     Invokes the middleware to handle global exceptions in the application.
    /// </summary>
    /// <param name="httpContext">
    ///     The HttpContext instance to use for the request.
    /// </param>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Could not process the request with TraceId: {TraceId}\n" +
                "Exception: {Exception}\n" +
                "TargetSite: {TargetSite}",
                Activity.Current?.Id,
                ex.Message,
                ex.TargetSite);

            await Results.Problem(
                    title: "An error occurred while processing your request.",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?>
                    {
                        {"traceId", Activity.Current?.Id},
                        {"message", ex.Message}
                    }
                )
                .ExecuteAsync(httpContext);
        }
    }
}
