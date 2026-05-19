using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.EF.Models
{
    public class TenantSettings
    {
        [Key]
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }
        public string PrimaryColor { get; set; } = "#000000";
        public string? LogoUrl { get; set; }
        public CurrencyType DefaultCurrency { get; set; } = CurrencyType.USD;
        public string TimeZone { get; set; } = "UTC";
        public bool EmailNotifications { get; set; } = true;
        public string? SlackWebhookUrl { get; set; }


        // Navigation Property
        public virtual Tenant Tenant { get; set; } = null!;
    }
}
