namespace Infrastructure.Data
{
    public class ApplicationTenantConfig
    {
        public ApplicationTenantClient[] Clients { get; set; }
    }

    public class ApplicationTenantClient
    {
        public string Name { get; set; }
        public string Organization { get; set; }
        public string[] Users { get; set; }
    }
}
