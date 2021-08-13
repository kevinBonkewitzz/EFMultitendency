using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Services
{
    public class TenantService : ITenantService
    {
        private static IList<ApplicationTenantClient> _tenants;

        public TenantService(IOptions<ApplicationTenantConfig> options)
        {
            _tenants = options.Value.Clients.ToList();
        }

        public ApplicationTenantClient GetTenant(string username)
        {
            // Check if the user is present in any client
            if (username != null)
                if (_tenants.Any(a => a.Users != null && a.Users.Any(s => s == username)))
                {
                    // Loop through clients to find which tenant this user is a part of
                    foreach (var tenant in _tenants.Where(x => x.Users != null))
                        if (tenant.Users.Any(a => a == username))
                            return _tenants.FirstOrDefault(f => f.Organization == tenant.Organization);
                }
            // The user is not present in any tenants- so default to the dev tenant
            return _tenants.FirstOrDefault(f => f.Organization == "");
        }
    }
}
