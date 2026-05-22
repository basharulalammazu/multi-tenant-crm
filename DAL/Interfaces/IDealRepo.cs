using DAL.EF.Models;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface IDealRepo : IRepository<Deal> 
    {
        public Deal? FindWithContact(Guid id, out string msg);
        public Deal? FindWithOwner(Guid id, out string msg);
        public Deal? FindWithActivities(Guid id, out string msg);
        public Deal? FindFull(Guid id, out string msg);
        public List<Deal> FindAllWithContact(out string msg);
        public List<Deal> FindAllWithOwner(out string msg);
        public List<Deal> FindByStage(DealStage stage, out string msg);
        public List<Deal> FindByContact(Guid contactId, out string msg);
        public List<Deal> FindByOwner(Guid ownerId, out string msg);
        public List<Deal> FindExpired(out string msg);
        public List<Deal> FindWonThisMonth(out string msg);
        public decimal SumValueByStage(DealStage stage, out string msg);
        public bool ChangeStage(Guid dealId, DealStage newStage, out string msg);
        public bool SoftDelete(Guid id, out string msg);

    }
}
