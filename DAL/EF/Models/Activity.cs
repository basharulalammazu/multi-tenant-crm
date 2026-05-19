using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.EF.Models
{
    public class Activity : BaseEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [ForeignKey("Contact")]
        public Guid? ContactId { get; set; }
        [ForeignKey("Deal")]
        public Guid? DealId { get; set; }
        public ActivityType Type { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime OccurredAt { get; set; }
        public int? DurationMin { get; set; }
        public bool IsDone { get; set; }
        public DateTime? DueAt { get; set; }


        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual AppUser User { get; set; } = null!;
        public virtual Contact? Contact { get; set; }
        public virtual Deal? Deal { get; set; }
    }
}
