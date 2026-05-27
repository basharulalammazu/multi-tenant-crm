using BLL.DTOs;
using BLL.DTOs.Shared;
using DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class UserService
    {
        private readonly DataAccessFactory factory;

        public UserService(DataAccessFactory factory)
        {
            this.factory = factory;
        }

        public UserDTO? GetById(Guid id, out string msg)
        {
            var user = factory.AppUserRepoAccess().FindByTenant(id, out msg);

            if (!string.IsNullOrEmpty(msg))
                return null;

            if (user == null) 
                throw new Exception("User not found");

            var mapper = MapperConfig.GetMapper();

            return mapper.Map<UserDTO>(user); 
        }
    }
}
