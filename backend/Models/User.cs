namespace PrendeteRock.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Design> Designs { get; set; } = new List<Design>();
    }
}