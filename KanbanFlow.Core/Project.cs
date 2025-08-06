using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanFlow.Core
{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int? UserId { get; set; } // Foreign Key for ownership
        public DateTime CreatedDate { get; set; }

        public byte[] RowVersion { get; set; } = [];

        public virtual User? User { get; set; }
        public virtual ICollection<Column> Columns { get; set; }

        public Project()
        {
            CreatedDate = DateTime.UtcNow;
            Columns = new HashSet<Column>();
        }

    }
}