using Dapper;
using DeepFakeWitness.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Data.SqlClient;
using System.Data;

namespace DeepFakeWitness.Controllers
{
      
    public class RegisterController : Controller
    {
        private readonly IConfiguration config;

        public RegisterController(IConfiguration config)
        {
            this.config = config;
        }
        public IActionResult Register()
        {
            return View();
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
             

                    var result = await connection.QuerySingleAsync<dynamic>("RegisterUser", parameter,
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
    }
}
