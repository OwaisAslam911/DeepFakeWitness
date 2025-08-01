using Microsoft.AspNetCore.Mvc;
using DeepFakeWitness.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Data;

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
           
            var conn = config.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(conn))
            {
                string query = "select * from Roles";
                var UserRoles = connection.Query(query).ToList();
                ViewBag.UserRoles = UserRoles;

            }
            return View();  
           
        } 
        public IActionResult DeactiveUsers()
        {
           
     
            return View();  
           
        }
        public JsonResult GetDeactiveUsers(Users users)
        {
            var conn = config.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(conn))
            {
                string UserQuery = "select * from Users u join UserRoles ur on ur.UserId = u.UserId join Roles r on ur.RoleId = r.RoleId Where IsActive = 0";
                var UserList = connection.Query(UserQuery).ToList(); // or map to a DTO if preferred
                return Json(new { data = UserList });
            }
        }
        public JsonResult GetUsers(Users users)
        {
            var conn = config.GetConnectionString("dbcs");
            using (var connection = new SqlConnection (conn))
            {
                string UserQuery = "select * from Users u join UserRoles ur on ur.UserId = u.UserId join Roles r on ur.RoleId = r.RoleId Where IsActive = 1";
                var UserList = connection.Query(UserQuery).ToList(); // or map to a DTO if preferred
                return Json(new { data = UserList });
            }
        }
        [HttpPost]
         public async Task<IActionResult> RegisterUser([FromBody]Users user)
        {
            try
            {


                var conn = config.GetConnectionString("dbcs");
                using (var connection = new SqlConnection(conn))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@UserName", user.UserName);
                    parameter.Add("@Email", user.Email);
                    parameter.Add("@UserPassword", user.UserPassword);
                    parameter.Add("@Phone", user.Phone);
                    parameter.Add("@RoleId", user.RoleId);


                    var result = await connection.QuerySingleAsync<dynamic>("[RegisterAdminAndUser]", parameter,
                commandType: CommandType.StoredProcedure
            );

                    return Json(new { status = result.Status, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        } 
        [HttpPost]
         public async Task<IActionResult> UpdateUser([FromBody]Users user)
        {
            try
            {


                var conn = config.GetConnectionString("dbcs");
                using (var connection = new SqlConnection(conn))
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@UserId", user.UserId);
                    parameter.Add("@UserName", user.UserName);
                    parameter.Add("@Email", user.Email);
                    parameter.Add("@UserPassword", user.UserPassword);
                    parameter.Add("@Phone", user.Phone);
                    parameter.Add("@RoleId", user.RoleId);


                    var result = await connection.QuerySingleAsync<dynamic>("[UpdateAdminAndUser]", parameter,
                commandType: CommandType.StoredProcedure
            );

                    return Json(new { status = result.Status, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult SoftDeleteUser(int userId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(config.GetConnectionString("dbcs")))
                {
                    using (SqlCommand cmd = new SqlCommand("SoftDeleteUser", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        con.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Json(new
                                {
                                    status = reader["Status"].ToString(),
                                    message = reader["Message"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }

            return Json(new { status = "error", message = "Unexpected error occurred." });
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
