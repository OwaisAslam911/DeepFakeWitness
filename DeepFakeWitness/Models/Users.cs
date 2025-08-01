namespace DeepFakeWitness.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserPassword { get; set; }
        public int IsActive { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
    }
}
