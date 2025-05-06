

using System.Diagnostics;
using Core;
using Serilog;
using Serilog.Context;

namespace Api.Middleware;

internal class GlobalExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            using (LogContext.PushProperty("CorrelationId", httpContext.TraceIdentifier))
            {
                await next(httpContext);
                Log.Information(
                    "Request {Endpoint} with TraceId: {TraceId} has been processed successfully.",
                    httpContext.GetEndpoint()?.DisplayName,
                    Activity.Current?.Id
                );
            }
        }
        catch (HttpNotFoundException ex)
        {
            await Results.NotFound(ex.Message)
                .ExecuteAsync(httpContext);
        }
        catch (UnauthorizedGetMyProfileException ex)
        {
            await Results.Problem(
                    title: "You are not authorized to get your profile.",
                    statusCode: StatusCodes.Status419AuthenticationTimeout
                )
                .ExecuteAsync(httpContext);
        }
        catch (UnauthorizedException ex)
        {
            await Results.Unauthorized()
                .ExecuteAsync(httpContext);
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "Could not process the request with TraceId: {TraceId}\n" +
                "Endpoint: {Endpoint}\n" +
                "Exception: {Exception}\n" +
                "TargetSite: {TargetSite}",
                Activity.Current?.Id,
                httpContext.GetEndpoint()?.DisplayName,
                ex.Message,
                ex.TargetSite);

            await Results.Problem(
                    title: "An error occurred while processing your request.",
                    statusCode: StatusCodes.Status400BadRequest,
                    extensions: new Dictionary<string, object?>
                    {
                        {"traceId", Activity.Current?.Id},
                        {"Endpoint", httpContext.GetEndpoint()?.DisplayName},
                        {"message", ex.Message}
                    }
                )
                .ExecuteAsync(httpContext);
        }
    }
}
