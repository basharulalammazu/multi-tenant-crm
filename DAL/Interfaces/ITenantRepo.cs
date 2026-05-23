using DAL.EF.Models;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface ITenantRepo
    {
        public Tenant? FindWithUsers(Guid id, out string msg);
        public Tenant? FindBySubdomain(string subdomain, out string msg);
        public List<Tenant> FindActive(out string msg);
        public List<Tenant> FindByPlan(PlanType plan, out string msg);
        public List<Tenant> FindTrialExpiringSoon(int days, out string msg);
        public bool Deactivate(Guid tenantId, out string msg);
        public bool ChangePlan(Guid tenantId, PlanType plan, out string msg);
    }
}
