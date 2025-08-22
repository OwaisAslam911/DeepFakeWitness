using DeepFakeWitness.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;

namespace DeepFakeWitness.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly DeepFakeWitnessContext _context;
        private readonly IConfiguration config;

        public UserProfileController(DeepFakeWitnessContext _context, IConfiguration config)
        {
            this._context = _context;
            this.config = config;
        }
        public IActionResult Profile()
        {
            return View();
        }





        [HttpGet]
        public JsonResult GetProfile(int userId)
        {
            using (var conn = new SqlConnection(config.GetConnectionString("dbcs")))
            {
                string query = "SELECT u.*, r.RoleName FROM Users u JOIN UserRoles ur ON u.UserId = ur.UserId JOIN Roles r ON ur.RoleId = r.RoleId where u.UserId = 2";
                var user = conn.QueryFirstOrDefault(query, new { UserId = userId });
                return Json(user);
            }
        }

        // Update profile info
        [HttpPost]
        public JsonResult UpdateProfile([FromBody] UserUpdateModel model)
        {
            using (var conn = new SqlConnection(config.GetConnectionString("dbcs")))
            {
                string query = @"UPDATE Users 
                             SET FullName=@FullName, Email=@Email, PasswordHash=@PasswordHash 
                             WHERE UserId=@UserId";
                int rows = conn.Execute(query, model);
                return Json(new { success = rows > 0 });
            }
        }

        // Upload history
        [HttpGet]
        public JsonResult GetUploadHistory(int userId)
        {
            using (var conn = new SqlConnection(config.GetConnectionString("dbcs")))
            {
                string query = @"SELECT UserId, FileName, DetectionResult, Id 
                         FROM UserImage 
                         WHERE UserId = @UserId 
                         ORDER BY Id DESC";

                var history = conn.Query(query, new { UserId = userId }).ToList();
                return Json(history);
            }
        }

    }
}

