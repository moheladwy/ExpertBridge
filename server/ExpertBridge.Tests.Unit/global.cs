// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using FluentValidation.TestHelper;
global using Shouldly;
global using Xunit;
global using ExpertBridge.Contract.Requests.MediaObject;
global using ExpertBridge.Contract.Requests.CreatePost;
global using ExpertBridge.Contract.Requests.EditPost;
global using ExpertBridge.Contract.Requests.CreateComment;
global using ExpertBridge.Contract.Requests.EditComment;
global using ExpertBridge.Contract.Requests.GeneratePresignedUrls;
global using ExpertBridge.Contract.Requests.ApplyToJobPosting;
global using ExpertBridge.Contract.Requests.PatchComment;
global using ExpertBridge.Contract.Requests.CreateJobPosting;
global using ExpertBridge.Contract.Requests.EditJobPosting;
global using ExpertBridge.Contract.Requests.CreateMessage;
global using ExpertBridge.Contract.Requests.CreateJobOffer;
global using ExpertBridge.Contract.Requests.SearchPost;
global using ExpertBridge.Contract.Requests.SearchUser;
global using ExpertBridge.Contract.Requests.SearchJobPosts;
global using ExpertBridge.Contract.Requests.InitiateJobOffer;
global using ExpertBridge.Contract.Requests.RespondToJobOffer;
global using ExpertBridge.Contract.Requests.UpdateJobStatus;
global using ExpertBridge.Contract.Requests.RegisterUser;
global using ExpertBridge.Contract.Requests.UpdateUserRequest;
global using ExpertBridge.Contract.Requests.UpdateProfileRequest;
global using ExpertBridge.Contract.Requests.OnboardUser;
global using ExpertBridge.Contract.Requests.UpdateProfileSkills;
global using ExpertBridge.Contract.Requests.PostsCursor;
global using ExpertBridge.Contract.Requests.JobPostingsPagination;
global using ExpertBridge.Core.Entities;
global using ExpertBridge.Core.Entities.Users;
global using ExpertBridge.Core.Entities.Profiles;
