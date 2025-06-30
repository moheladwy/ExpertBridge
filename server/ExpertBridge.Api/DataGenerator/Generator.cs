// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Bogus;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.JobStatuses;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Data.DatabaseContexts;
using Pgvector;

namespace ExpertBridge.Api.DataGenerator;

public static class Generator
{
    public static List<User> GenerateUsers(int count)
    {
        var userFaker = new Faker<User>()
            .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
            .RuleFor(u => u.ProviderId, f => f.Random.Guid().ToString())
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("01013647953").OrNull(f, 0.3f))
            .RuleFor(u => u.IsBanned, f => f.Random.Bool(0.05f))
            .RuleFor(u => u.IsDeleted, f => f.Random.Bool(0.02f))
            .RuleFor(u => u.IsEmailVerified, f => true)
            .RuleFor(u => u.IsOnboarded, f => false)
            .RuleFor(u => u.CreatedAt, f => f.Date.Past(2).ToUniversalTime());

        return userFaker.Generate(count);
    }

    public static List<Profile> GenerateProfiles(List<User> users)
    {
        var profileFaker = new Faker<Profile>()
            .RuleFor(p => p.Id, f => Guid.NewGuid().ToString())
            .RuleFor(p => p.UserId, f => f.PickRandom(users).Id)
            .RuleFor(p => p.JobTitle, f => f.Name.JobTitle().OrNull(f, 0.2f))
            .RuleFor(p => p.Bio, f => f.Lorem.Sentence().OrNull(f, 0.3f))
            .RuleFor(p => p.ProfilePictureUrl, f => f.Internet.Avatar().OrNull(f, 0.2f))
            .RuleFor(p => p.Rating, f => f.Random.Double(1, 5))
            .RuleFor(p => p.RatingCount, f => f.Random.Int(0, 100))
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.PhoneNumber, f => f.PickRandom(users).PhoneNumber.OrNull(f, 0.3f))
            .RuleFor(p => p.FirstName, f => f.PickRandom(users).FirstName)
            .RuleFor(p => p.LastName, f => f.PickRandom(users).LastName)
            ;

        return profileFaker.Generate(users.Count);
    }

    private static readonly Random random = new Random();
    public static Vector GenerateRandomVector(int dimensions)
    {
        return new Vector(GenerateRandomVectorArray(dimensions));
    }
    public static float[] GenerateRandomVectorArray(int dimensions)
    {
        var values = new float[dimensions];
        for (var i = 0; i < dimensions; i++)
        {
            values[i] = (float)(random.NextDouble() * 2 - 1);
        }

        return values;
    }

    public static List<Post> GeneratePosts(List<Profile> profiles, int count)
    {
        var postFaker = new Faker<Post>()
            .RuleFor(p => p.Id, f => Guid.NewGuid().ToString())
            .RuleFor(p => p.Author, f => f.PickRandom(profiles))
            //.RuleFor(p => p.AuthorId, f => f.PickRandom(profiles).Id)
            .RuleFor(p => p.Title, f => f.Lorem.Sentence())
            .RuleFor(p => p.Content, f => f.Lorem.Paragraph())
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(p => p.LastModified, f => f.Date.Recent(10).ToUniversalTime().OrNull(f, 0.3f))
            .RuleFor(p => p.IsDeleted, f => f.Random.Bool(0.1f))
            .RuleFor(p => p.Embedding, f => GenerateRandomVector(1024))
            ;

        return postFaker.Generate(count);
    }

    public static List<Post> GeneratePosts(string profileId, int count)
    {
        var postFaker = new Faker<Post>()
            .RuleFor(p => p.Id, f => Guid.NewGuid().ToString())
            .RuleFor(p => p.AuthorId, f => profileId)
            .RuleFor(p => p.Title, f => f.Lorem.Sentence(wordCount: 10))
            .RuleFor(p => p.Content, f => f.Lorem.Sentence(wordCount: 20))
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(1, DateTime.UtcNow).ToUniversalTime())
            .RuleFor(p => p.IsDeleted, f => false)
            .RuleFor(p => p.Embedding, f => GenerateRandomVector(1024))
            ;

        return postFaker.Generate(count);
    }

    public static List<Comment> GenerateComments(List<Profile> profiles, List<Post> posts, int count)
    {
        var commentFaker = new Faker<Comment>()
            .RuleFor(c => c.Id, f => Guid.NewGuid().ToString())
            .RuleFor(c => c.Author, f => f.PickRandom(profiles))
            .RuleFor(c => c.Post, f => f.PickRandom(posts))
            .RuleFor(c => c.Content, f => f.Lorem.Sentence())
            .RuleFor(c => c.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(c => c.IsDeleted, f => false);

        return commentFaker.Generate(count);
    }



    public static List<Job> GenerateJobs(List<Profile> profiles, int count)
    {
        var jobFaker = new Faker<Job>()
            .RuleFor(j => j.Id, f => Guid.NewGuid().ToString())
            .RuleFor(j => j.Author, f => f.PickRandom(profiles))
            .RuleFor(j => j.Worker, f => f.PickRandom(profiles))
            .RuleFor(j => j.StartedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(j => j.Status, f => f.PickRandom<JobStatusEnum>());

        return jobFaker.Generate(count);
    }

    public static void SeedDatabase(ExpertBridgeDbContext context)
    {
        var users = GenerateUsers(50);
        var profiles = GenerateProfiles(users);
        //var jobs = GenerateJobs(profiles, 50);

        context.Users.AddRange(users);
        context.Profiles.AddRange(profiles);
        context.SaveChanges();

        var posts = GeneratePosts(profiles, 1000000);
        context.Posts.AddRange(posts);
        context.SaveChanges();
        //var posts = context.Posts.ToList();

        //var comments = GenerateComments(profiles, posts, 200);
        //context.Comments.AddRange(comments);
        //context.SaveChanges();

        //context.JobStatuses.AddRange(jobStatuses);
        //context.SaveChanges();

        //context.Jobs.AddRange(jobs);
        //context.SaveChanges();
    }
}
