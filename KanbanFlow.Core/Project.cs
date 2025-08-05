using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanFlow.Core
{
    public class Project
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? UserId { get; set; } // Foreign Key for ownership

        public byte[] RowVersion { get; set; } = [];

        public virtual User? User { get; set; }
        public virtual ICollection<Column> Columns { get; set; }

        public Project()
        {
            Columns = new HashSet<Column>();
        }

    }
}