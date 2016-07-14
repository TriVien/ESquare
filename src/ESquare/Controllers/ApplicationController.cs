using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESquare.Multitenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESquare.Controllers
{
    public class ApplicationController : Controller
    {
        protected ApplicationContext AppContext;

        public ApplicationController(ApplicationContext context)
        {
            AppContext = context;
        }
    }
}
