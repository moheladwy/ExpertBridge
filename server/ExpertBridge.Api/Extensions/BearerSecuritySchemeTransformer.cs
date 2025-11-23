// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ExpertBridge.Api.Extensions;

/// <summary>
/// The <c>BearerSecuritySchemeTransformer</c> class is an implementation of the <c>IOpenApiDocumentTransformer</c> interface
/// that configures an OpenAPI document to include and enforce the "Bearer" security scheme.
/// </summary>
/// <remarks>
/// This transformer adds a bearer token-based security scheme to the OpenAPI document. It checks whether an authentication
/// scheme named "Bearer" is registered, and if so, adds a corresponding security scheme to the document's components. The
/// transformer also applies the required security configuration to operations within the document.
/// </remarks>
/// <example>
/// This class is typically registered as an OpenAPI document transformer in the application's service configuration.
/// </example>
/// <param name="authenticationSchemeProvider">
/// The <c>IAuthenticationSchemeProvider</c> instance used to access the registered authentication schemes in the application.
/// </param>
internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // "bearer" refers to the header name here
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = securitySchemes;

            // Apply it as a requirement for all operations
            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
            {
                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }
        }
    }
}
