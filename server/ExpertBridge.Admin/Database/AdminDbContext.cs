// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Admin.Database;

public sealed class AdminDbContext : IdentityDbContext<Admin>
{
    public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options)
    {
    }
}
