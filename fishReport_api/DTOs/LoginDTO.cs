namespace FishReportApi.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? Role { get; set; }
    }

    public class LoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}