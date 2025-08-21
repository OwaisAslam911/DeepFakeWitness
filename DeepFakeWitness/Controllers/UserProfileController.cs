using Microsoft.AspNetCore.Mvc;

namespace DeepFakeWitness.Controllers
{
    public class UserProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
