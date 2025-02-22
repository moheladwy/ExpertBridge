using System.Diagnostics;
using Serilog;
using Serilog.Context;

namespace ExpertBridge.Api.Middlewares;

internal class GlobalExceptionMiddleware(RequestDelegate next)
{
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
            using (LogContext.PushProperty("CorrelationId", httpContext.TraceIdentifier))
            {
                await next(httpContext);
                Log.Information(
                    "Request with TraceId: {TraceId} has been processed successfully.",
                    Activity.Current?.Id);
            }
        }
        catch (Exception ex)
        {
            Log.Error(
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
