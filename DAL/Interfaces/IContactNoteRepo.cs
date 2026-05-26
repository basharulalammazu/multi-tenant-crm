using DAL.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface IContactNoteRepo : IRepository<ContactNote>
    {
        public List<ContactNote> FindByContact(Guid contactId, out string msg);
        public List<ContactNote> FindByContactWithContact(Guid contactId, out string msg);
        public ContactNote? FindWithContact(Guid id, out string msg);
        public bool SoftDelete(Guid id, out string msg);
    }
}
