using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;

namespace DeepFakeWitness.Controllers
{
    public class AdminScanResultController : Controller
    {
        private readonly IConfiguration config;
        private readonly DeepFakeWitnessContext context;

        public AdminScanResultController(IConfiguration config, DeepFakeWitnessContext _context)
        {
            this.config = config;
            context = _context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAllResults(int userId = 0) // default if not passed
        {
            using (var conn = new SqlConnection(config.GetConnectionString("dbcs")))
            {
                string query = @"SELECT 
                            u.UserId, 
                            u.UserName, 
                            ui.FileName, 
                            ui.DetectionResult 
                         FROM Users u 
                         JOIN UserImage ui ON u.UserId = ui.UserId ORDER BY ui.Id DESC";

                var history = conn.Query(query).ToList();

                // DataTables expects { data: [...] }
                return Json(new { data = history });
            }
        }
 



    }
}
