// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ExpertBridgeDbContext _dbContext;

        public TagsController(ExpertBridgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<List<TagResponse>> GetAll()
        {
            var tags = await _dbContext.Tags
                .Select(tag => new TagResponse
                {
                    TagId = tag.Id,
                    EnglishName = tag.EnglishName,
                    ArabicName = tag.ArabicName,
                    Description = tag.Description,
                })
                .ToListAsync();

            return tags;
        }
    }
}
