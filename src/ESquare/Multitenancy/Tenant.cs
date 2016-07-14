using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESquare.Multitenancy
{
    public class Tenant
    {
        public string Name { get; set; }

        public List<string> Hostnames { get; set; }

        public string Theme { get; set; }
    }
}
