using System.ComponentModel.DataAnnotations;

namespace KanbanFlow.Core.Users
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public byte[] RowVersion { get; set; } = [];

        public User()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}
