using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DeepFakeWitness.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

namespace DeepFakeWitness.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly DeepFakeWitnessContext _context;
        private readonly IConfiguration config;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, DeepFakeWitnessContext context, IConfiguration config)
        {
            _logger = logger;
            _env = env;
            _context = context;
            this.config = config;
        }
        public IActionResult Index()
        {
            //var conn = config.GetConnectionString("dbcs");
            //using(var connection = new SqlConnection(conn))
            //{
            //    var role = HttpContext.Session.GetString("UserRole");
            //if (role == "Admin")
            //{
            //    ViewData["Layout"] = "AdminLayout";
            //}
            //else
            //{
            //    ViewData["Layout"] = "_Layout";
            //}
            //return View();
            //}
            return View();
        }
        //[HttpPost]
        //public async Task<JsonResult> CheckImage(IFormFile image)
        //{
        //    if (image != null && image.Length > 0)
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //        string uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
        //        if (!Directory.Exists(uploadsPath))
        //            Directory.CreateDirectory(uploadsPath);

        //        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        //        string fullImagePath = Path.Combine(uploadsPath, uniqueFileName);

        //        using (var stream = new FileStream(fullImagePath, FileMode.Create))
        //        {
        //            await image.CopyToAsync(stream);
        //        }

        //        string result = RunPythonDetection(fullImagePath);

        //        var userImage = new UserImage
        //        {
        //            FileName = uniqueFileName,
        //            UserId = userId,
        //            DetectionResult = result
        //        };
        //        _context.UserImage.Add(userImage);
        //        await _context.SaveChangesAsync();

        //        return Json(new { success = true, message = result });
        //    }

        //    return Json(new { success = false, message = "Please upload a valid image." });
        //}

        [HttpPost]
        public async Task<IActionResult> CheckImage(IFormFile image)
         {
            if (image != null && image.Length > 0)
            {
                // 1. Get user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // 2. Create uploads folder if not exists
                string uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                // 3. Save image
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string fullImagePath = Path.Combine(uploadsPath, uniqueFileName);

                using (var stream = new FileStream(fullImagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // 4. Run Python detection
                string result = RunPythonDetection(fullImagePath);

                // 5. Save to database
                var userImage = new UserImage
                {
                    FileName = uniqueFileName,
                    UserId = userId,
                    DetectionResult = result
                };
                _context.UserImage.Add(userImage);
                _context.SaveChanges();

                // 6. Show result
                ViewBag.ImagePath = "/uploads/" + uniqueFileName;
                ViewBag.Result = result;

                return View("Index");
            }

            ModelState.AddModelError("", "Please upload a valid image.");
            return RedirectToAction("Index");
        }

        private string RunPythonDetection(string imagePath)
        {
            var pythonPath = @"C:\Users\AA\AppData\Local\Programs\Python\Python313\python.exe";
            var scriptPath = Path.Combine(_env.ContentRootPath, "detect.py");

            _logger.LogInformation($"Python script path: {scriptPath}");

            var startInfo = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = $"\"{scriptPath}\" \"{imagePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    _logger.LogError($"Python Error: {error}");
                    return "Detection failed";
                }

                return output.Trim(); // "Deepfake" or "Real"
            }
        }
        public IActionResult Contact()
        {
            return View();
        }   
        public IActionResult About()
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
