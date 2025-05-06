using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Profiles;

namespace Core.Entities.ModerationReports
{
    public class ModerationReport : BaseModel, ISoftDeletable
    {
        public ContentTypes ContentType { get; set; } // Enum for content type (e.g., Post, Comment, etc.)
        public string ContentId { get; set; }
        public string AuthorId { get; set; }
        public string Reason { get; set; }
        public bool IsResolved { get; set; }
        public bool IsNegative { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Results
        public double Toxicity { get; set; }
        public double SevereToxicity { get; set; }
        public double Obscene { get; set; }
        public double Threat { get; set; }
        public double Insult { get; set; }
        public double IdentityAttack { get; set; }
        public double SexualExplicit { get; set; }

        // Navigation properties
    }
}
