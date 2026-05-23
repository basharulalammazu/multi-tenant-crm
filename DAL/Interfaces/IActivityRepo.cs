using DAL.EF.Models;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface IActivityRepo
    {
        public Activity? FindWithContact(Guid id, out string msg);
        public Activity? FindWithDeal(Guid id, out string msg);
        public Activity? FindFull(Guid id, out string msg);
        public List<Activity> FindByContact(Guid contactId, out string msg);
        public List<Activity> FindByDeal(Guid dealId, out string msg);
        public List<Activity> FindByType(ActivityType type, out string msg);
        public List<Activity> FindByDateRange(DateTime from, DateTime to, out string msg);
        public List<Activity> FindByCreatedBy(string email, out string msg);
        public bool SoftDelete(Guid id, out string msg);
    }
}
