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
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector.EntityFrameworkCore;

namespace ExpertBridge.Application.DomainServices;

public class ProfileService
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly ILogger<ProfileService> _logger;
    private readonly IValidator<UpdateProfileRequest> _updateProfileRequestValidator;

    public ProfileService(
        ExpertBridgeDbContext dbContext,
        ILogger<ProfileService> logger,
        IValidator<UpdateProfileRequest> updateProfileRequestValidator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _updateProfileRequestValidator = updateProfileRequestValidator;
    }

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
