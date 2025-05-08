using ExpertBridge.Core.Entities.Media;
using ExpertBridge.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Data.Queries
{
    public static class MediaObjectQueries
    {
        // WARNING!
        // This method uses a hardcoded URL for the media object.
        // This is a magic string that will be a pain in the ass to maintain
        // and cause dependent modules to rebuild on change.
        public static IQueryable<MediaObjectResponse> SelectMediaObjectResponse(
            this IQueryable<MediaObject> query)
        {
            return query
                .AsNoTracking()
                .Select(m => new MediaObjectResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Type = m.Type,
                    Url = $"https://expert-bridge-media.s3.amazonaws.com/{m.Key}"
                });
        }
    }
}
