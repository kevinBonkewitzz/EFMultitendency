using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Linq;
using Infrastructure.Interfaces;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _tenantConnStr;
        private IDbConnection DbConnection { get; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            ITenantService tenantProvider,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            var connectionStrings = configuration.GetSection("ConnectionStrings").GetChildren();
            var tenantConnectionStrings = connectionStrings.Where(s => s.Key.StartsWith("ConnectionTenant")).ToArray();

            if (httpContextAccessor.HttpContext != null)
            {
                var headers = httpContextAccessor.HttpContext.Request.Headers;

                // Retrieve the UserId
                if (headers.Any(x => x.Key == "user"))
                {
                    // In real app we would get the userId from the token here
                    var user = headers.FirstOrDefault(x => x.Key == "user");

                    // Retrieve the organization for the current user 
                    var tenant = tenantProvider.GetTenant(user.Value);
                    var lookup = "ConnectionTenant" + tenant.Organization;
                    _tenantConnStr = tenantConnectionStrings.FirstOrDefault(x => x.Key == lookup).Value;
                }
                // No token specified - so we default to the dev tenant
                else
                    _tenantConnStr = tenantConnectionStrings.FirstOrDefault().Value;
            }
            else
                _tenantConnStr = tenantConnectionStrings.FirstOrDefault().Value;
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_tenantConnStr != null)
                optionsBuilder.UseSqlServer(_tenantConnStr);
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
