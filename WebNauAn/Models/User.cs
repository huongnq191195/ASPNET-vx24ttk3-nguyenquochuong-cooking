namespace WebNauAn.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // Dùng để phân biệt "Admin" và "Thành Viên"
        public string Role { get; set; }
    }
}