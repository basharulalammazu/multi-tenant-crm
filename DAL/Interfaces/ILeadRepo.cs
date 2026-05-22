using DAL.EF.Models;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface ILeadRepo
    {
        public Lead? FindWithAssignedUser(Guid id, out string msg);
        public List<Lead> FindAllWithAssignedUser(out string msg);
        public List<Lead> FindByStatus(LeadStatus status, out string msg);
        public List<Lead> FindByAssignedUser(Guid userId, out string msg);
        public List<Lead> FindUnassigned(out string msg);
        public List<Lead> FindConverted(out string msg);
        public List<Lead> FindBySource(string source, out string msg);
        public List<Lead> FindStalled(int daysWithoutActivity, out string msg);
        public int CountByStatus(LeadStatus status, out string msg);
        public bool AssignToUser(Guid leadId, Guid userId, out string msg);
        public bool MarkConverted(Guid leadId, out string msg);
        public bool SoftDelete(Guid id, out string msg);

    }
}
