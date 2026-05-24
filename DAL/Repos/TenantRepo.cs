using DAL.EF;
using DAL.EF.Models;
using DAL.Enums;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repos
{
    public class TenantRepo : Repository<Tenant>, ITenantRepo
    {
        private readonly CrmSaaSDbContext db;
        public TenantRepo(CrmSaaSDbContext db) : base(db)
        {
            this.db = db;
        }

        public bool ChangePlan(Guid tenantId, PlanType plan, out string msg)
        {
            try
            {
                var tenant = GetById(tenantId, out msg);
                if (tenant == null)
                {
                    msg = "Tenant not found.";
                    return false;
                }

                tenant.Plan = plan;

                if (plan == PlanType.Free)
                    tenant.TrialEndsAt = null;


                return Update(tenant, out msg);
            }
            catch(Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        public bool Deactivate(Guid tenantId, out string msg)
        {
            try
            {
                var tenant = GetById(tenantId, out msg);

                if (tenant == null)
                {
                    msg = "Tenant not found.";
                    return false;
                }

                tenant.IsActive = false;
                return Update(tenant, out msg);
            }
            catch(Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        public List<Tenant> FindActive(out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Tenants.Where(x => x.IsActive)
                                    .OrderBy(x => x.Name).ToList();
            }
            catch(Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public List<Tenant> FindByPlan(PlanType plan, out string msg)
        {
            throw new NotImplementedException();
        }

        public Tenant? FindBySubdomain(string subdomain, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Tenants.FirstOrDefault(x => x.Subdomain == subdomain);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;    
            }
        }

        public List<Tenant> FindTrialExpiringSoon(int days, out string msg)
        {
            try
            {
                msg = string.Empty;
                var cutOff = DateTime.UtcNow.AddDays(days);

                return db.Tenants.Where(x => x.IsActive
                                        && x.Plan == PlanType.Free
                                        && x.TrialEndsAt != null
                                        && x.TrialEndsAt <= cutOff
                                        && x.TrialEndsAt >= DateTime.UtcNow)
                                .OrderBy(x => x.TrialEndsAt).ToList();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }

        public Tenant? FindWithUsers(Guid id, out string msg)
        {
            try
            {
                msg = string.Empty;
                return db.Tenants.Include(x => x.Users).FirstOrDefault(x => x.Id == id);
            }

            catch(Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }
    }
}
