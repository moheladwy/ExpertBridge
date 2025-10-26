using System.Globalization;
using ExpertBridge.Application.DataGenerator;
using ExpertBridge.Core.Entities.ManyToManyRelationships.ProfileSkills;
using ExpertBridge.Core.Entities.Profiles;
using ExpertBridge.Core.Entities.Skills;
using ExpertBridge.Core.Entities.Users;
using ExpertBridge.Core.Exceptions;
using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Requests;
using ExpertBridge.Core.Requests.UpdateProfileRequest;
using ExpertBridge.Core.Requests.UpdateProfileSkills;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector.EntityFrameworkCore;

namespace ExpertBridge.Application.DomainServices;

/// <summary>
/// Provides comprehensive profile management including updates, skill management, and AI-powered recommendations.
/// </summary>
/// <remarks>
/// This service manages user profiles with focus on professional information, skills, and matching algorithms
/// for connecting experts based on similarity and reputation.
/// </remarks>
public class ProfileService
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly ILogger<ProfileService> _logger;
    private readonly IValidator<UpdateProfileRequest> _updateProfileRequestValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for profile and skill operations.</param>
    /// <param name="logger">The logger for diagnostic information.</param>
    /// <param name="updateProfileRequestValidator">The validator for profile update requests.</param>
    public ProfileService(
        ExpertBridgeDbContext dbContext,
        ILogger<ProfileService> logger,
        IValidator<UpdateProfileRequest> updateProfileRequestValidator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _updateProfileRequestValidator = updateProfileRequestValidator;
    }

    /// <summary>
    /// Retrieves profiles similar to the user based on AI vector embeddings of skills and interests.
    /// </summary>
    /// <param name="userProfile">
    /// The profile to find similar profiles for. Can be null for anonymous discovery.
    /// </param>
    /// <param name="limit">The maximum number of similar profiles to return.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of similar <see cref="ProfileResponse"/> objects.
    /// </returns>
    /// <remarks>
    /// **Similarity Algorithm:**
    /// Uses cosine distance between UserInterestEmbedding vectors:
    /// - Distance 0.0 = identical interests
    /// - Distance 1.0 = orthogonal (no overlap)
    /// - Distance 2.0 = opposite interests
    ///
    /// **Vector Generation:**
    /// - User embeddings generated from interaction history (posts, votes, applications)
    /// - Background worker aggregates tags into embedding
    /// - 1024-dimension vector (mxbai-embed-large model)
    ///
    /// **Anonymous Users:**
    /// If userProfile is null, generates random embedding for exploration.
    ///
    /// **Example:**
    /// <code>
    /// // Find experts similar to current user
    /// var similar = await _profileService.GetSimilarProfilesAsync(userProfile, 10);
    /// // Returns top 10 profiles with closest skill/interest match
    /// </code>
    ///
    /// **Use Cases:**
    /// - Expert discovery ("Find people like me")
    /// - Network expansion suggestions
    /// - Collaboration recommendations
    /// - Mentorship matching
    /// </remarks>
    public async Task<List<ProfileResponse>> GetSimilarProfilesAsync(
        Profile? userProfile,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var userEmbedding = userProfile?.UserInterestEmbedding ?? Generator.GenerateRandomVector(1024);

        var query = _dbContext.Profiles
            .AsNoTracking()
            .FullyPopulatedProfileQuery()
            .AsQueryable();

        if (userProfile != null)
        {
            query = query.Where(p => p.Id != userProfile.Id);
        }

        var suggested = await query
            .Where(p => p.UserInterestEmbedding != null)
            .Take(limit)
            .OrderBy(p => p.UserInterestEmbedding.CosineDistance(userEmbedding))
            .SelectProfileResponseFromProfile()
            .ToListAsync(cancellationToken);

        return suggested;
    }

    /// <summary>
    /// Retrieves profiles with the highest reputation scores, excluding the current user.
    /// </summary>
    /// <param name="userProfile">
    /// The current user's profile to exclude from results. Can be null for anonymous browsing.
    /// </param>
    /// <param name="limit">The maximum number of top profiles to return.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of top <see cref="ProfileResponse"/> objects
    /// ordered by reputation score descending.
    /// </returns>
    /// <remarks>
    /// **Reputation Calculation:**
    /// Reputation is derived from:
    /// - Upvotes on posts and comments
    /// - Completed jobs
    /// - Positive ratings
    /// - Community contributions
    ///
    /// **Sorting:**
    /// Profiles ordered by Reputation property (higher = better).
    ///
    /// **Use Cases:**
    /// - "Top Experts" leaderboard
    /// - Featured professional showcase
    /// - Highlighting platform success stories
    /// - Building trust through social proof
    ///
    /// **Example:**
    /// <code>
    /// var topExperts = await _profileService.GetTopReputationProfilesAsync(null, 20);
    /// // Returns 20 highest-reputation profiles on the platform
    /// </code>
    /// </remarks>
    public async Task<List<ProfileResponse>> GetTopReputationProfilesAsync(
        Profile? userProfile,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Profiles
            .FullyPopulatedProfileQuery()
            .AsQueryable();

        if (userProfile != null)
        {
            query = query.Where(p => p.Id != userProfile.Id);
        }

        var topProfiles = await query
            .Take(limit)
            .SelectProfileResponseFromProfile()
            .ToListAsync(cancellationToken);

        return topProfiles.OrderByDescending(p => p.Reputation).ToList();
    }

    /// <summary>
    /// Updates a user's profile information including personal details and skills.
    /// </summary>
    /// <param name="user">The user entity whose profile is being updated.</param>
    /// <param name="request">The profile update request containing new field values.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the updated <see cref="ProfileResponse"/>.
    /// </returns>
    /// <exception cref="ValidationException">
    /// Thrown when the request fails validation (invalid format, missing required fields).
    /// </exception>
    /// <exception cref="ProfileUserNameAlreadyExistsException">
    /// Thrown when the requested username is already taken by another user.
    /// </exception>
    /// <exception cref="ProfilePhoneNumberAlreadyExistsException">
    /// Thrown when the requested phone number is already registered to another user.
    /// </exception>
    /// <exception cref="ProfileNotFoundException">
    /// Thrown when the profile cannot be found after update (data consistency issue).
    /// </exception>
    /// <remarks>
    /// **Update Strategy:**
    /// Only non-null/non-empty fields in request are updated (partial update pattern).
    ///
    /// **Uniqueness Validation:**
    /// - Username: Case-sensitive, must be unique across all profiles
    /// - PhoneNumber: Must be unique across all profiles
    ///
    /// **Updatable Fields:**
    /// - Username (unique constraint)
    /// - PhoneNumber (unique constraint)
    /// - FirstName, LastName
    /// - Bio (professional summary)
    /// - JobTitle (current position)
    /// - Skills (managed separately via UpdateProfileSkillsAsync)
    ///
    /// **Validation Rules:**
    /// Enforced by FluentValidation:
    /// - FirstName/LastName: Max 50 characters
    /// - Username: 3-50 characters, alphanumeric + underscore
    /// - PhoneNumber: Valid E.164 format
    /// - Bio: Max 500 characters
    /// - JobTitle: Max 100 characters
    ///
    /// **Skill Management:**
    /// Skills are updated via internal UpdateProfileSkillsAsync:
    /// - Normalizes skill names (lowercase, trimmed)
    /// - Removes duplicates
    /// - Creates new skills if not exist
    /// - Maintains ProfileSkill relationships
    ///
    /// **Example:**
    /// <code>
    /// var updated = await _profileService.UpdateProfileAsync(user, new UpdateProfileRequest
    /// {
    ///     FirstName = "John",
    ///     LastName = "Doe",
    ///     Bio = "Senior software engineer",
    ///     JobTitle = "Lead Developer",
    ///     Skills = new List&lt;string&gt; { "C#", ".NET", "Azure" }
    /// });
    /// </code>
    ///
    /// **Transaction Safety:**
    /// All updates (profile fields + skills) committed atomically in single SaveChanges.
    ///
    /// **Post-Update:**
    /// Profile is re-fetched with all navigation properties for complete response.
    /// </remarks>
    public async Task<ProfileResponse> UpdateProfileAsync(
        User user,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(request);
        // Validate the request using the validator from FluentValidation.
        var validationResult = await _updateProfileRequestValidator
            .ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var profile = user.Profile;

        if (!string.IsNullOrEmpty(request.Username) &&
            request.Username != user.Profile.Username)
        {
            var isUsernameExists = await _dbContext.Profiles
                .AsNoTracking()
                .AnyAsync(p => p.Username == request.Username && p.Username != user.Profile.Username,
                    cancellationToken);
            if (isUsernameExists)
            {
                throw new ProfileUserNameAlreadyExistsException($"Username '{user.Profile.Username}' already exists.");
            }

            profile.Username = request.Username;
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber) &&
            request.PhoneNumber != user.Profile.PhoneNumber)
        {
            var isPhoneNumberExisting = await _dbContext.Profiles
                .AsNoTracking()
                .AnyAsync(p =>
                        p.PhoneNumber == request.PhoneNumber && p.PhoneNumber != user.Profile.PhoneNumber,
                    cancellationToken);
            if (isPhoneNumberExisting)
            {
                throw new ProfilePhoneNumberAlreadyExistsException(
                    $"Phone number '{request.PhoneNumber}' already exists.");
            }

            profile.PhoneNumber = request.PhoneNumber;
        }

        if (!string.IsNullOrEmpty(request.FirstName) && request.FirstName != user.Profile.FirstName)
        {
            profile.FirstName = request.FirstName;
        }

        if (!string.IsNullOrEmpty(request.LastName) && request.LastName != user.Profile.LastName)
        {
            profile.LastName = request.LastName;
        }

        if (!string.IsNullOrEmpty(request.Bio) && request.Bio != user.Profile.Bio)
        {
            profile.Bio = request.Bio;
        }

        if (!string.IsNullOrEmpty(request.JobTitle) && request.JobTitle != user.Profile.JobTitle)
        {
            profile.JobTitle = request.JobTitle;
        }

        await UpdateProfileSkillsAsync(
            profile,
            new UpdateProfileSkillsRequest { Skills = request.Skills },
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var profileResponse = await _dbContext.Profiles
            .FullyPopulatedProfileQuery(p => p.Id == user.Profile.Id)
            .SelectProfileResponseFromProfile()
            .FirstOrDefaultAsync(cancellationToken);

        if (profileResponse == null)
        {
            throw new ProfileNotFoundException($"User[{user.Id}] Profile was not found");
        }

        return profileResponse;
    }

    /// <summary>
    /// Internal method that updates a profile's skills with normalization, deduplication, and relationship management.
    /// </summary>
    /// <param name="profile">The profile entity to update skills for.</param>
    /// <param name="request">The skills update request containing the new skill list.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// **Normalization Process:**
    /// 1. Trim whitespace from skill names
    /// 2. Convert to lowercase (culture-invariant)
    /// 3. Remove duplicates
    /// 4. Filter out empty/null entries
    ///
    /// **Skill Creation:**
    /// - Query existing skills by name
    /// - Create new Skill entities for names not in database
    /// - Add new skills to Skills table
    ///
    /// **Relationship Management:**
    /// - Load existing ProfileSkill relationships
    /// - Add ProfileSkills for new skills
    /// - Remove ProfileSkills for skills not in new list
    /// - Maintains referential integrity
    ///
    /// **Example Flow:**
    /// <code>
    /// Request: ["Python", "Python ", "PYTHON", "Java"]
    /// Normalized: ["python", "java"] (deduplicated, lowercase)
    ///
    /// Existing in DB: ["python"]
    /// New to create: ["java"]
    ///
    /// Existing ProfileSkills: [python, c#]
    /// To add: [java]
    /// To remove: [c#]
    ///
    /// Final ProfileSkills: [python, java]
    /// </code>
    ///
    /// **Why Lowercase:**
    /// - Case-insensitive matching ("Python" = "python")
    /// - Consistent skill taxonomy
    /// - Prevents duplicate skills with different casing
    ///
    /// **Transaction Context:**
    /// Changes are tracked but not committed. Caller must call SaveChangesAsync.
    ///
    /// This is an internal method and should not be called directly from controllers.
    /// </remarks>
    private async Task UpdateProfileSkillsAsync(
        Profile profile,
        UpdateProfileSkillsRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(request);

        // Normalize skills: trim whitespace, convert to lowercase, and remove duplicates
        request.Skills = request.Skills
            .Where(skill => !string.IsNullOrWhiteSpace(skill))
            .Select(skill => skill.Trim().ToLower(CultureInfo.InvariantCulture))
            .Distinct()
            .ToList();

        // get the exiting skills from the database.
        var existingSkills = await _dbContext.Skills
            .AsNoTracking()
            .Where(skill => request.Skills.Contains(skill.Name))
            .ToListAsync(cancellationToken);

        var skillsToBeAdded = request.Skills
            .Where(skill => existingSkills
                .All(existingSkill => !existingSkill.Name.Equals(skill, StringComparison.OrdinalIgnoreCase)))
            .Select(skillName => new Skill { Name = skillName })
            .ToList();

        await _dbContext.Skills.AddRangeAsync(skillsToBeAdded, cancellationToken);

        var allSkills = existingSkills.Concat(skillsToBeAdded).ToList();

        var existingProfileSkills = await _dbContext.ProfileSkills
            .Include(ps => ps.Skill)
            .Where(ps => ps.ProfileId == profile.Id)
            .ToListAsync(cancellationToken);

        var profileSkillsToBeAdded = request.Skills
            .Where(skill => existingProfileSkills
                .All(existingSkill => !existingSkill.Skill.Name
                    .Equals(skill, StringComparison.OrdinalIgnoreCase)))
            .Select(skillName => new ProfileSkill
            {
                ProfileId = profile.Id,
                SkillId = allSkills
                    .First(s => s.Name.Equals(skillName, StringComparison.OrdinalIgnoreCase))
                    .Id
            })
            .ToList();

        var profileSkillsToBeRemoved = existingProfileSkills
            .Where(ps => !request.Skills.Contains(ps.Skill.Name))
            .ToList();

        _dbContext.ProfileSkills.RemoveRange(profileSkillsToBeRemoved);

        await _dbContext.ProfileSkills.AddRangeAsync(profileSkillsToBeAdded, cancellationToken);
    }
}
