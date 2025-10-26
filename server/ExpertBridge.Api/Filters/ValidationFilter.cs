// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExpertBridge.Api.Filters;

/// <summary>
/// Action filter that automatically validates request models using FluentValidation validators.
/// </summary>
/// <remarks>
/// This filter runs before controller actions and validates all request parameters that have
/// registered FluentValidation validators. If validation fails, it returns a 400 Bad Request
/// with detailed error information.
///
/// **Usage:**
/// Register globally in Program.cs:
/// <code>
/// builder.Services.AddControllers(options =>
/// {
///     options.Filters.Add&lt;ValidationFilter&gt;();
/// });
/// </code>
///
/// **How It Works:**
/// 1. Inspects all action parameters
/// 2. Attempts to resolve IValidator&lt;T&gt; for each parameter type
/// 3. If validator exists, validates the parameter
/// 4. If validation fails, short-circuits the request with error response
///
/// **Error Response Format:**
/// <code>
/// {
///   "title": "Validation Failed",
///   "status": 400,
///   "errors": {
///     "PropertyName": ["Error message 1", "Error message 2"]
///   },
///   "traceId": "..."
/// }
/// </code>
/// </remarks>
public sealed class ValidationFilter : IAsyncActionFilter
{
  private readonly IServiceProvider _serviceProvider;

  public ValidationFilter(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    ArgumentNullException.ThrowIfNull(context);
    ArgumentNullException.ThrowIfNull(next);

    // Iterate through all action parameters
    foreach (var parameter in context.ActionDescriptor.Parameters)
    {
      var parameterType = parameter.ParameterType;

      // Skip primitive types and types from System namespace
      if (parameterType.IsPrimitive ||
          parameterType.IsEnum ||
          parameterType == typeof(string) ||
          parameterType.Namespace?.StartsWith("System", StringComparison.Ordinal) == true)
      {
        continue;
      }

      // Try to get the validator for this parameter type
      var validatorType = typeof(IValidator<>).MakeGenericType(parameterType);
      var validator = _serviceProvider.GetService(validatorType) as IValidator;

      if (validator == null)
      {
        // No validator registered for this type, skip
        continue;
      }

      // Get the actual parameter value from the context
      if (!context.ActionArguments.TryGetValue(parameter.Name, out var parameterValue) || parameterValue == null)
      {
        // Parameter not provided or null, skip (let controller handle null validation)
        continue;
      }

      // Create validation context
      var validationContext = new ValidationContext<object>(parameterValue);

      // Validate asynchronously
      var validationResult = await validator.ValidateAsync(validationContext);

      if (!validationResult.IsValid)
      {
        // Validation failed - build error response
        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var problemDetails = new
        {
          Title = "Validation Failed",
          Status = 400,
          Errors = errors,
          TraceId = context.HttpContext.TraceIdentifier
        };

        context.Result = new BadRequestObjectResult(problemDetails)
        {
          StatusCode = StatusCodes.Status400BadRequest
        };

        return; // Short-circuit the pipeline
      }
    }

    // All validations passed, continue to action
    await next();
  }
}
