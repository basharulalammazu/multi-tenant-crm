using DAL.EF;
using DAL.Interfaces;
using DAL.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public class DataAccessFactory
    {
        private CrmSaaSDbContext db;

        public DataAccessFactory(CrmSaaSDbContext db)
        {
            this.db = db;
        }

        // Generic repository access
        public IRepository<T> GetRepo<T>() where T : class
        {
            return new Repository<T>(db);
        }

        // Feature specific repository
        public IActivityRepo ActivityRepoAccess()
        {
            return new ActivityRepo(db);
        }

        public IAppUserRepo AppUserRepoAccess()
        {
            return new AppUserRepo(db);
        }

        public IAuditLogRepo AuditLogRepoAccess()
        {
            return new AuditLogRepo(db);
        }

        public IContactNoteRepo ContactNoteRepoAccess()
        {
            return new ContactNoteRepo(db);
        }

        public IContactRepo ContactRepoAccess()
        {
            return new ContactRepo(db);
        }

        public IDealRepo DealRepoAccess()
        {
            return new DealRepo(db);
        }

        public ILeadRepo LeadRepoAccess()
        {
            return new LeadRepo(db);
        }

        public INotificationRepo NotificationRepoAccess()
        {
            return new NotificationRepo(db);
        }

        public ITenantRepo TenantRepoAccess()
        {
            return new TenantRepo(db);
        }

    }
}
