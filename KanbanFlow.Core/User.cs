namespace KanbanFlow.Core
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
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