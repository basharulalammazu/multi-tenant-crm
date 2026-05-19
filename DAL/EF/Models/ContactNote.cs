using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.EF.Models
{
    public class ContactNote : BaseEntity
    {
        [ForeignKey("Contact")]
        public Guid ContactId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsPinned { get; set; }


        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual Contact Contact { get; set; } = null!;
    }
}
