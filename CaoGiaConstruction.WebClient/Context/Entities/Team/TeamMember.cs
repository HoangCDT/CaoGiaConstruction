using System.ComponentModel.DataAnnotations;

namespace CaoGiaConstruction.WebClient.Context.Entities
{
    public class TeamMember : EntityBase
    {
        [StringLength(255)]
        public string FullName { get; set; }

        [StringLength(255)]
        public string Position { get; set; } // e.g., "CEO & Founder"

        [StringLength(512)]
        public string Avatar { get; set; }

        [StringLength(1024)]
        public string Quote { get; set; } // For the Founder section or bio

        public int SortOrder { get; set; }

        public bool IsFounder { get; set; } // To distinguish the Founder section
    }
}
