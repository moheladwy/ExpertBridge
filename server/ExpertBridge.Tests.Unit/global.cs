// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using FluentValidation.TestHelper;
global using Shouldly;
global using Xunit;
global using ExpertBridge.Core.Requests.MediaObject;
global using ExpertBridge.Core.Requests.CreatePost;
global using ExpertBridge.Core.Requests.EditPost;
global using ExpertBridge.Core.Requests.CreateComment;
global using ExpertBridge.Core.Requests.EditComment;
global using ExpertBridge.Core.Requests.GeneratePresignedUrls;
global using ExpertBridge.Core.Requests.ApplyToJobPosting;
global using ExpertBridge.Core.Requests.PatchComment;
global using ExpertBridge.Core.Requests.CreateJobPosting;
global using ExpertBridge.Core.Requests.EditJobPosting;
global using ExpertBridge.Core.Requests.CreateMessage;
global using ExpertBridge.Core.Requests.CreateJobOffer;
global using ExpertBridge.Core.Requests.SearchPost;
global using ExpertBridge.Core.Requests.SearchUser;
global using ExpertBridge.Core.Requests.SearchJobPosts;
global using ExpertBridge.Core.Requests.InitiateJobOffer;
global using ExpertBridge.Core.Requests.RespondToJobOffer;
global using ExpertBridge.Core.Requests.UpdateJobStatus;
global using ExpertBridge.Core.Requests.RegisterUser;
global using ExpertBridge.Core.Requests.UpdateUserRequest;
global using ExpertBridge.Core.Requests.UpdateProfileRequest;
global using ExpertBridge.Core.Requests.OnboardUser;
global using ExpertBridge.Core.Requests.UpdateProfileSkills;
global using ExpertBridge.Core.Requests.PostsCursor;
global using ExpertBridge.Core.Requests.JobPostingsPagination;
global using ExpertBridge.Core.Entities;