using DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class TenantManagementService
    {
        private DataAccessFactory factory;
        public TenantManagementService(DataAccessFactory factory) 
        {
            this.factory = factory;
        }
    }
}
