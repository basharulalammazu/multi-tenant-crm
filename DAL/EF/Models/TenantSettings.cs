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

        [Required]
        [StringLength(20)]
        [Column(TypeName = "VARCHAR(20)")]
        public string PrimaryColor { get; set; } = "#000000";

        [StringLength(500)]
        [Column(TypeName = "VARCHAR(500)")]
        public string? LogoUrl { get; set; }
        public CurrencyType DefaultCurrency { get; set; } = CurrencyType.USD;

        [Required]
        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string TimeZone { get; set; } = "UTC";
        public bool EmailNotifications { get; set; } = true;

        [StringLength(500)]
        [Column(TypeName = "VARCHAR(500)")]
        public string? SlackWebhookUrl { get; set; }


        // Navigation Property
        public virtual Tenant Tenant { get; set; } = null!;
    }
}
