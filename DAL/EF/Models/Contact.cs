using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DAL.EF.Models
{
    public class Contact : BaseEntity
    {
        public Guid? LeadId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? CompanyName { get; set; }

        public string? JobTitle { get; set; }

        public string? Website { get; set; }

        public string? Notes { get; set; }

        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;

        public virtual Lead? Lead { get; set; }

        public virtual ICollection<ContactNote> ContactNotes { get; set; } = new List<ContactNote>();

        public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();

        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
