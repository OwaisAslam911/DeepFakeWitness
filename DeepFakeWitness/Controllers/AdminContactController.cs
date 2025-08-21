using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace DeepFakeWitness.Controllers
{
    public class AdminContactController : Controller
    {
        private readonly IConfiguration config;
        private readonly DeepFakeWitnessContext _context;

        public AdminContactController(IConfiguration config, DeepFakeWitnessContext _context)
        {
            this.config = config;
            this._context = _context;
        }

        public JsonResult GetAllMessages()
        {
            var conn = config.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(conn))
            {
                string query = "SELECT * FROM Contact ORDER BY MessageId DESC";
                var result = connection.Query(query);
                return Json(new { data = result }); // DataTables expects { data: [...] }
            }
        }
        [HttpPost]
        public JsonResult DeleteMessage(int messageId)
        {
            var conn = config.GetConnectionString("dbcs");
            using (var connection = new SqlConnection(conn))
            {
                string query = "DELETE FROM Contact WHERE MessageId = @MessageId";
                var rows = connection.Execute(query, new { MessageId = messageId });

                return Json(new { success = rows > 0 });
            }
        }
        [HttpGet]
        public JsonResult GetNewMessagesCount()
        {
            try
            {
                var con = config.GetConnectionString("dbcs");
                using (var connection = new SqlConnection(con))
                {
                    string query = "SELECT COUNT(*) FROM Contact WHERE IsRead = 0;";
                    var result = connection.ExecuteScalar<int>(query); // Dapper one-liner
                    return Json(new { newMessages = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { newMessages = 0, error = ex.Message });
            }
        }

        // ✅ Show messages in admin panel
       

        // ✅ Mark a message as read
        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            try
            {
                var message = _context.Contact.Find(id);
                if (message != null)
                {
                    message.IsRead = true;
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, error = "Message not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
