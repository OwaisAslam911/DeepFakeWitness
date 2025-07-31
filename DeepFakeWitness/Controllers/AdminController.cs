using Microsoft.AspNetCore.Mvc;
using DeepFakeWitness.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DeepFakeWitness.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration config;

        public AdminController(IConfiguration config)
        {
            this.config = config;
        }
        public IActionResult Dashboard()
        {
            try
            {
                var conn = config.GetConnectionString("dbcs");

                using (var connection = new SqlConnection(conn))
                {
                    string query = "SELECT COUNT(*) FROM Users";
                    int userCount = connection.ExecuteScalar<int>(query);

                    ViewBag.UserCount = userCount;
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.UserCount = "Error";
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }








        public IActionResult Users()
        {
            return View();
        }
         public JsonResult GetUsers(Users users)
        {
            var conn = config.GetConnectionString("dbcs");
            using (var connection = new SqlConnection (conn))
            {
                string UserQuery = "Select * from Users";
                var result = connection.QueryFirstOrDefault(UserQuery);
            return Json(result);
            }
        }



        public IActionResult Profile()
        {
            return View();
        }  
        public IActionResult RegisterNewAdmin()
        {
            return View();
        }
    }
}
