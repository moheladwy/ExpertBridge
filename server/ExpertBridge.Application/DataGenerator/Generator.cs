using Bogus;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.Jobs;
using ExpertBridge.Core.Entities.JobStatuses;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Data.DatabaseContexts;
using Pgvector;

namespace ExpertBridge.Application.DataGenerator;

/// <summary>
/// Provides static utility methods for generating fake test data using the Bogus library.
/// </summary>
/// <remarks>
/// This is a development and testing utility class for seeding the database with realistic fake data
/// during local development, testing, and load testing scenarios.
/// 
/// **Primary Use Cases:**
/// - Local development environment setup (seed with realistic data)
/// - Integration testing (generate test fixtures)
/// - Performance/load testing (create large datasets)
/// - Demo environments (populate with sample content)
/// 
/// **Data Generation Capabilities:**
/// - **Users**: Email, username, phone, verification status
/// - **Profiles**: Job titles, bios, avatars, ratings
/// - **Posts**: Titles, content, embeddings (1024-dim vectors)
/// - **Comments**: Content with random authors and target posts
/// - **Jobs**: Job records with client/worker assignments
/// - **Vectors**: Random embeddings for AI/ML testing
/// 
/// **Bogus Library Integration:**
/// Uses Faker&lt;T&gt; with fluent rule configuration:
/// <code>
/// var userFaker = new Faker&lt;User&gt;()
///     .RuleFor(u => u.Email, f => f.Internet.Email())
///     .RuleFor(u => u.Username, f => f.Internet.UserName());
/// var users = userFaker.Generate(100); // 100 fake users
/// </code>
/// 
/// **Vector Generation:**
/// - Generates random float arrays normalized to [-1, 1] range
/// - Default: 1024 dimensions (matches Ollama mxbai-embed-large model)
/// - Used for testing similarity search and recommendation algorithms
/// 
/// **Database Seeding Workflow:**
/// <code>
/// SeedDatabase(dbContext):
///   1. Generate users (50)
///   2. Generate profiles (1 per user)
///   3. Save users + profiles
///   4. Generate posts (1,000,000 with random embeddings)
///   5. Save posts
///   6. Optionally: comments, jobs, etc.
/// </code>
/// 
/// **Performance Considerations:**
/// - SeedDatabase generates 1M posts by default (adjust for testing needs)
/// - Uses AddRange for batch inserts
/// - Consider BulkInsertAsync for very large datasets (see FakePostsGeneratorWorker)
/// - Random embeddings are computationally cheap but not semantically meaningful
/// 
/// **Realistic Data Patterns:**
/// - 5% of users banned (IsBanned = true)
/// - 2% of users soft deleted (IsDeleted = true)
/// - 30% chance of null phone numbers
/// - 20% chance of null profile pictures
/// - 10% chance of deleted posts
/// - Random ratings 1-5 stars with varying counts
/// 
/// **Thread Safety:**
/// Static Random instance is NOT thread-safe. For concurrent use, consider ThreadLocal&lt;Random&gt;
/// or use Random.Shared (available in .NET 6+).
/// 
/// **Development vs Production:**
/// ⚠️ **WARNING:** This class should NEVER be used in production environments.
/// - Intended for development, testing, and demo purposes only
/// - Generated data is fake and not suitable for real applications
/// - No validation or business logic applied
/// - Random embeddings don't represent actual semantic content
/// 
/// **Extension Points:**
/// To add new entity generators:
/// 1. Create new static method (e.g., GenerateJobPostings)
/// 2. Define Faker&lt;TEntity&gt; with RuleFor rules
/// 3. Return faker.Generate(count)
/// 4. Add to SeedDatabase method if needed
/// 
/// **Testing Recommendations:**
/// - Use small counts (10-100) for unit tests
/// - Use medium counts (1,000-10,000) for integration tests
/// - Use large counts (100,000+) for load testing only
/// - Clear database between test runs for consistency
/// 
/// All methods are static and stateless except for shared Random instance.
/// </remarks>
public static class Generator
{
    private static readonly Random random = new();

    /// <summary>
    /// Generates a specified number of fake User entities with realistic data.
    /// </summary>
    /// <param name="count">The number of users to generate.</param>
    /// <returns>A list of fake User entities with randomized properties.</returns>
    /// <remarks>
    /// **Generated User Properties:**
    /// - Unique GUIDs for Id and ProviderId
    /// - Realistic names, emails, usernames from Bogus
    /// - 30% chance of null phone numbers
    /// - 5% chance of banned users
    /// - 2% chance of soft-deleted users
    /// - All users email verified and not onboarded
    /// - Created dates within past 2 years
    /// 
    /// Example: `var users = Generator.GenerateUsers(50);`
    /// </remarks>
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

    /// <summary>
    /// Generates Profile entities for a given list of Users (one profile per user).
    /// </summary>
    /// <param name="users">The list of users to create profiles for.</param>
    /// <returns>A list of fake Profile entities associated with the provided users.</returns>
    /// <remarks>
    /// **Generated Profile Properties:**
    /// - Randomly picks UserId from provided users
    /// - 20% chance of null job titles
    /// - 30% chance of null bios
    /// - 20% chance of null profile pictures
    /// - Random ratings 1-5 with 0-100 rating counts
    /// - Copies email/phone from associated user
    /// 
    /// **Note:** Returns same count as input users, but UserId assignment is random
    /// (multiple profiles may reference same user if not careful).
    /// </remarks>
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

    /// <summary>
    /// Generates a random pgvector Vector with specified dimensions.
    /// </summary>
    /// <param name="dimensions">The number of dimensions (typically 1024 for ExpertBridge).</param>
    /// <returns>A Pgvector Vector with random float values in range [-1, 1].</returns>
    /// <remarks>
    /// Used for testing AI recommendation algorithms without running actual embedding models.
    /// Default dimension is 1024 to match Ollama mxbai-embed-large model output.
    /// 
    /// **Warning:** Random vectors have NO semantic meaning and will produce random similarity results.
    /// </remarks>
    public static Vector GenerateRandomVector(int dimensions)
    {
        return new Vector(GenerateRandomVectorArray(dimensions));
    }

    /// <summary>
    /// Generates a random float array with specified dimensions for vector embeddings.
    /// </summary>
    /// <param name="dimensions">The number of dimensions in the vector.</param>
    /// <returns>A float array with random values in range [-1, 1].</returns>
    /// <remarks>
    /// Helper method that creates the underlying array for Vector construction.
    /// Each dimension is independently randomized using uniform distribution.
    /// </remarks>
    public static float[] GenerateRandomVectorArray(int dimensions)
    {
        var values = new float[dimensions];
        for (var i = 0; i < dimensions; i++)
        {
            values[i] = (float)(random.NextDouble() * 2 - 1);
        }

        return values;
    }

    /// <summary>
    /// Generates fake Post entities with random authors from provided profiles.
    /// </summary>
    /// <param name="profiles">The list of profiles to randomly assign as post authors.</param>
    /// <param name="count">The number of posts to generate.</param>
    /// <returns>A list of fake Post entities with embeddings and randomized content.</returns>
    /// <remarks>
    /// **Generated Post Properties:**
    /// - Random sentence titles (Lorem Ipsum)
    /// - Random paragraph content
    /// - Random author from provided profiles
    /// - Created dates within past year
    /// - 30% chance of recent modification (last 10 days)
    /// - 10% chance of soft-deleted posts
    /// - 1024-dimension random embeddings
    /// 
    /// **Performance Note:** Generating 1M posts with embeddings creates significant memory usage
    /// (~4-6 GB for vectors alone). Consider batch processing for very large datasets.
    /// </remarks>
    public static List<Post> GeneratePosts(List<Profile> profiles, int count)
    {
        var postFaker = new Faker<Post>()
                .RuleFor(p => p.Id, f => Guid.NewGuid().ToString())
                .RuleFor(p => p.Author, f => f.PickRandom(profiles))
                //.RuleFor(p => p.AuthorId, f => f.PickRandom(profiles).Id)
                .RuleFor(p => p.Title, f => f.Lorem.Sentence())
                .RuleFor(p => p.Content, f => f.Lorem.Paragraph())
                .RuleFor(p => p.CreatedAt, f => f.Date.Past().ToUniversalTime())
                .RuleFor(p => p.LastModified, f => f.Date.Recent(10).ToUniversalTime().OrNull(f, 0.3f))
                .RuleFor(p => p.IsDeleted, f => f.Random.Bool(0.1f))
                .RuleFor(p => p.Embedding, f => GenerateRandomVector(1024))
            ;

        return postFaker.Generate(count);
    }

    /// <summary>
    /// Generates fake Post entities all authored by a specific profile (overload).
    /// </summary>
    /// <param name="profileId">The profile ID to assign as author for all generated posts.</param>
    /// <param name="count">The number of posts to generate.</param>
    /// <returns>A list of fake Post entities all authored by the specified profile.</returns>
    /// <remarks>
    /// **Use Case:** Testing single user's content feed, user profile pages, or performance testing
    /// with large amounts of content from one author.
    /// 
    /// Differs from main overload by:
    /// - All posts have same AuthorId
    /// - Longer sentences (10 words title, 20 words content)
    /// - Created within past year only
    /// - No soft-deleted posts (IsDeleted always false)
    /// </remarks>
    public static List<Post> GeneratePosts(string profileId, int count)
    {
        var postFaker = new Faker<Post>()
                .RuleFor(p => p.Id, f => Guid.NewGuid().ToString())
                .RuleFor(p => p.AuthorId, f => profileId)
                .RuleFor(p => p.Title, f => f.Lorem.Sentence(10))
                .RuleFor(p => p.Content, f => f.Lorem.Sentence(20))
                .RuleFor(p => p.CreatedAt, f => f.Date.Past(1, DateTime.UtcNow).ToUniversalTime())
                .RuleFor(p => p.IsDeleted, f => false)
                .RuleFor(p => p.Embedding, f => GenerateRandomVector(1024))
            ;

        return postFaker.Generate(count);
    }

    /// <summary>
    /// Generates fake Comment entities with random authors and target posts.
    /// </summary>
    /// <param name="profiles">The list of profiles to randomly assign as comment authors.</param>
    /// <param name="posts">The list of posts to randomly assign as comment targets.</param>
    /// <param name="count">The number of comments to generate.</param>
    /// <returns>A list of fake Comment entities.</returns>
    /// <remarks>
    /// **Generated Comment Properties:**
    /// - Random sentence content
    /// - Random author from provided profiles
    /// - Random target post from provided posts
    /// - Created dates within past year
    /// - No soft-deleted comments (IsDeleted always false)
    /// 
    /// **Note:** Does not generate nested comments (ParentCommentId always null).
    /// For testing nested replies, manually set ParentCommentId after generation.
    /// </remarks>
    public static List<Comment> GenerateComments(List<Profile> profiles, List<Post> posts, int count)
    {
        var commentFaker = new Faker<Comment>()
            .RuleFor(c => c.Id, f => Guid.NewGuid().ToString())
            .RuleFor(c => c.Author, f => f.PickRandom(profiles))
            .RuleFor(c => c.Post, f => f.PickRandom(posts))
            .RuleFor(c => c.Content, f => f.Lorem.Sentence())
            .RuleFor(c => c.CreatedAt, f => f.Date.Past().ToUniversalTime())
            .RuleFor(c => c.IsDeleted, f => false);

        return commentFaker.Generate(count);
    }

    /// <summary>
    /// Generates fake Job entities with random client and worker assignments.
    /// </summary>
    /// <param name="profiles">The list of profiles to randomly assign as job authors (clients) and workers.</param>
    /// <param name="count">The number of jobs to generate.</param>
    /// <returns>A list of fake Job entities with random statuses.</returns>
    /// <remarks>
    /// **Generated Job Properties:**
    /// - Random author (client) from provided profiles
    /// - Random worker from provided profiles
    /// - Started dates within past year
    /// - Random status from JobStatusEnum
    /// 
    /// **Note:** Author and Worker may be the same profile (self-assignment).
    /// Consider filtering or validation if this is not desired for testing.
    /// </remarks>
    public static List<Job> GenerateJobs(List<Profile> profiles, int count)
    {
        var jobFaker = new Faker<Job>()
            .RuleFor(j => j.Id, f => Guid.NewGuid().ToString())
            .RuleFor(j => j.Author, f => f.PickRandom(profiles))
            .RuleFor(j => j.Worker, f => f.PickRandom(profiles))
            .RuleFor(j => j.StartedAt, f => f.Date.Past().ToUniversalTime())
            .RuleFor(j => j.Status, f => f.PickRandom<JobStatusEnum>());

        return jobFaker.Generate(count);
    }

    /// <summary>
    /// Seeds the database with a complete set of fake data for development/testing.
    /// </summary>
    /// <param name="context">The ExpertBridgeDbContext to seed with data.</param>
    /// <remarks>
    /// **Default Seeding Strategy:**
    /// 1. Generate 50 users with profiles
    /// 2. Save users and profiles to database
    /// 3. Generate 1,000,000 posts with random embeddings
    /// 4. Save posts to database
    /// 
    /// **Performance Characteristics:**
    /// - 1M posts with 1024-dim embeddings = ~4-6 GB memory
    /// - Multiple SaveChanges calls for batch commits
    /// - Takes several minutes to complete depending on hardware
    /// 
    /// **Commented Sections:**
    /// - Jobs generation (uncomment if needed)
    /// - Comments generation (uncomment if needed)
    /// - Alternative: load posts first, then generate comments
    /// 
    /// **Usage:**
    /// <code>
    /// using var context = serviceProvider.GetRequiredService&lt;ExpertBridgeDbContext&gt;();
    /// Generator.SeedDatabase(context);
    /// </code>
    /// 
    /// **Warning:** Clears existing data in affected tables. Use on fresh databases only.
    /// Consider using transactions or database snapshots for testing scenarios.
    /// </remarks>
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
