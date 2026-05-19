using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.EF.Models
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public PlanType Plan { get; set; } = PlanType.Free;
        public bool IsActive { get; set; } = true;
        public DateTime? TrialEndsAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        // Navigation Properties
        public virtual TenantSettings? Settings { get; set; }
        public virtual ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
        public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public virtual ICollection<Deal> Deals { get; set; } = new List<Deal>();
        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

}
