using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Interfaces
{
    public interface ITenantService
    {
        ApplicationTenantClient GetTenant(string userId);
    }
}
