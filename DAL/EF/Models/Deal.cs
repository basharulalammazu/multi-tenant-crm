using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.EF.Models
{
    public class Deal : BaseEntity
    {
        [ForeignKey("Contact")]
        public Guid ContactId { get; set; }
        [ForeignKey("Owner")]
        public Guid OwnerId { get; set; }

        [Required]
        [StringLength(150)]
        [Column(TypeName = "VARCHAR(150)")]
        public string Title { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public CurrencyType Currency { get; set; } = CurrencyType.USD;
        public DealStage Stage { get; set; } = DealStage.Prospect;
        public DateTime? ExpectedCloseAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        [StringLength(1000)]
        [Column(TypeName = "VARCHAR(1000)")]
        public string? Notes { get; set; }


        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual Contact Contact { get; set; } = null!;
        public virtual AppUser Owner { get; set; } = null!;
        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}