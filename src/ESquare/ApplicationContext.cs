using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESquare.Multitenancy;

namespace ESquare
{
    public class ApplicationContext
    {
        public Tenant CurrentTenant { get; private set; }

        public ApplicationContext(Tenant tenant)
        {
            CurrentTenant = tenant;
        }
    }
}
