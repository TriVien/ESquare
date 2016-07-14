using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESquare.Multitenancy;
using Microsoft.AspNetCore.Mvc;

namespace ESquare.Controllers
{
    public class HomeController : ApplicationController
    {
        public HomeController(ApplicationContext context) : base(context)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
