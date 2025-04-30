// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Api.Models.IPC;

namespace ExpertBridge.Api.Extensions
{
    public static class IpcChannels
    {
        public static WebApplicationBuilder AddIpcChannels(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddSingleton(_ => Channel.CreateUnbounded<PostCreatedMessage>())
                .AddSingleton(_ => Channel.CreateUnbounded<UserInterestsUpdatedMessage>())
                .AddSingleton(_ => Channel.CreateUnbounded<EmbedPostMessage>())
                ;

            return builder;
        }
    }
}
