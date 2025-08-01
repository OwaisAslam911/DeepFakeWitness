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


        [HttpGet]
        public IActionResult GetProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Json(new { status = "error", message = "User not logged in" });

            var user = GetUserById(userId.Value);
            if (user == null)
                return Json(new { status = "error", message = "User not found" });

            return Json(new { status = "success", data = user });
        }

        private Users GetUserById(int userId)
        {
            Users user = null;

            using (SqlConnection conn = new SqlConnection(config.GetConnectionString("dbcs")))
            {
                string query = @"SELECT u.UserId, u.Username, u.Email, u.Phone, r.RoleName
                             FROM Users u
                             JOIN UserRoles ur ON u.UserId = ur.UserId
                             JOIN Roles r ON ur.RoleId = r.RoleId
                             WHERE u.UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    user = new Users
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        UserName = reader["Username"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        RoleName = reader["RoleName"].ToString()
                    };
                }
            }

            return user;
        }
        public IActionResult RegisterNewAdmin()
        {
            return View();
        }
    }
}
