using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T GetById(Guid id, out string msg);
        List<T> GetAll(out string msg);
        bool Add(T entity, out string msg);
        bool Update(T entity, out string msg);
        bool Delete(Guid id, out string msg);
    }
}
