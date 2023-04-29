using Microsoft.AspNetCore.Mvc;

namespace AskerSite_.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
