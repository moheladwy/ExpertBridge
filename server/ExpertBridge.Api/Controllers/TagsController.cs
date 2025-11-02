// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Contract.Queries;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers;

/// <summary>
///     API controller for managing tags in the application.
/// </summary>
/// <remarks>
///     This controller provides endpoints for managing and retrieving tags.
///     It is secured with authorization and exposes operations such as fetching all available tags.
/// </remarks>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class TagsController : ControllerBase
{
    /// <summary>
    ///     Instance of <see cref="ExpertBridgeDbContext" /> used to interact with the database layer.
    /// </summary>
    /// <remarks>
    ///     This field provides access to the database context for performing operations
    ///     such as querying and manipulating entities related to tags and other resources within the application.
    ///     It is injected via dependency injection and is essential for managing database transactions and accessing data.
    /// </remarks>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     API controller for managing tags in the application.
    /// </summary>
    /// <remarks>
    ///     This controller provides endpoints for operations related to tags,
    ///     including retrieving all the available tags in the system.
    ///     It is secured with an authorization requirement and interacts with the database
    ///     using the provided <see cref="ExpertBridgeDbContext" />.
    /// </remarks>
    public TagsController(ExpertBridgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    ///     Retrieves a list of all available tags.
    /// </summary>
    /// <remarks>
    ///     This method queries the database to fetch a collection of tags and maps them to a list of
    ///     <see cref="TagResponse" />.
    ///     It utilizes the database context <see cref="ExpertBridgeDbContext" /> and custom query functionality defined in
    ///     <see cref="TagQueries" />.
    /// </remarks>
    /// <returns>
    ///     A task representing the asynchronous operation, with a result of type <see cref="List{TagResponse}" /> containing
    ///     all tags.
    /// </returns>
    [HttpGet]
    public async Task<List<TagResponse>> GetAll()
    {
        var tags = await _dbContext.Tags
            .SelectTagResponseFromTag()
            .ToListAsync();

        return tags;
    }
}
