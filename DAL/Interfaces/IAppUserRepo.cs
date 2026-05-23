using DAL.EF.Models;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface IAppUserRepo
    {
        public AppUser? FindWithTenant(Guid id, out string msg);
        public AppUser? FindByEmail(string email, out string msg);
        public AppUser? FindByRefreshToken(string refreshToken, out string msg);
        public List<AppUser> FindAllWithTenant(out string msg);
        public List<AppUser> FindByRole(UserRole role, out string msg);
        public List<AppUser> FindByTenant(Guid tenantId, out string msg);
        public bool UpdateRefreshToken(Guid userId, string? token, DateTime? expiry, out string msg);
        public bool Deactivate(Guid userId, out string msg);

    }
}
