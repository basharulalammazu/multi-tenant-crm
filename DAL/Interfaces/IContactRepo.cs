using DAL.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface IContactRepo
    {
        public Contact? FindWithDeals(Guid id, out string msg);
        public Contact? FindWithNotes(Guid id, out string msg);
        public Contact? FindWithActivities(Guid id, out string msg);
        public Contact? FindFull(Guid id, out string msg); // Contact + all related data (deals, notes, activities, origin lead
        public List<Contact> FindAllWithDeals(out string msg);
        public List<Contact> FindByCompany(string company, out string msg);
        public List<Contact> FindByLead(Guid leadId, out string msg);
        public List<Contact> FindWithOpenDeals(out string msg); // Contacts that have at least one open (non-Won/Lost) 
        public bool SoftDelete(Guid id, out string msg);  // Soft delete
    }
}
