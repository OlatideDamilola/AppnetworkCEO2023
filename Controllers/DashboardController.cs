using Microsoft.AspNetCore.Mvc;

namespace AppnetworkCEO2023.Controllers {
    public class DashboardController : Controller {
        public IActionResult Ceoindex() {
          var oo=  TempData["Tmpdata"];
            return View();
            
        } 
        public IActionResult Shareholderindex() {
            return View();
        }
    }
}
