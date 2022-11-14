using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MaskingWebApp.Models;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.Services.DataMasking;

namespace MaskingWebApp.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [EnforcerAuthorization]
        public IActionResult Index()
        {
            return View(new ResponseViewModel()
            {
                Message = "Top secret can't see this",
                From = "Andy",
                To = "Sally"
            });
        }
        
         [EnforcerAuthorization]
                public IActionResult Many()
                {
                    return View(Enumerable.Range(0,10).Select(i=> new ResponseViewModel()
                    {
                        Message = "Top secret can't see this",
                        From = "Andy",
                        To = "Sally"
                    }).ToList());
                }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}