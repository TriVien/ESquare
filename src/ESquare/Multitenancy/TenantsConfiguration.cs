using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ESquare.Multitenancy
{
    public class TenantsConfiguration
    {
        public List<Tenant> Tenants { get; set; }
    }
}
