namespace Pharmacy_System.DTOs.User
{
    public class UserResponseDto
    {

        public int UserID { get; set; }     

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
