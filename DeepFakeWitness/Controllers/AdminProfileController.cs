using Microsoft.AspNetCore.Mvc;

namespace DeepFakeWitness.Controllers
{
    public class AdminProfileController : Controller
    {
        public IActionResult AdminProfile()
        {
            return View();
        }
    }
}
