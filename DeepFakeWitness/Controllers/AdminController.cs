using Microsoft.AspNetCore.Mvc;

namespace DeepFakeWitness.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
