using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.EF.Models
{
    public class Lead : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? Source { get; set; }
        public LeadStatus Status { get; set; }
        [ForeignKey("AssignedToUser")]
        public Guid? AssignedToUserId { get; set; }
        public string? Notes { get; set; }
        public DateTime? LastContactedAt { get; set; }
        public DateTime? ConvertedAt { get; set; }


        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual AppUser? AssignedToUser { get; set; }
        public virtual Contact? Contact { get; set; }
    }
}
