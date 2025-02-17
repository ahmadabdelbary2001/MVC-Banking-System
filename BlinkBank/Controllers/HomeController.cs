using BlinkBank.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace BlinkBank.Controllers
{
    public class HomeController : Controller
    {

        BankDBContext db;
        public HomeController(BankDBContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}