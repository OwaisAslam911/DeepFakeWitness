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

    public class UserImage
    {
        public int Id { get; set; }

        public string FileName { get; set; }
        public int UserId { get; set; }
        public string DetectionResult { get; set; } // Optional
    }

    public class UserProfile
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }

    public class UploadHistory
    {
        public int UploadId { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }  // Authentic / Deepfake / Suspicious
    }
    public class UserUpdateModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserPassword { get; set; }
    }
}
