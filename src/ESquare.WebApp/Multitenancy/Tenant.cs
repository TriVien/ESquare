using System.Collections.Generic;

namespace ESquare.WebApp.Multitenancy
{
    public class Tenant
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<string> Hostnames { get; set; }

        public string Theme { get; set; }
    }
}
