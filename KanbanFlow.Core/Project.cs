using System.ComponentModel.DataAnnotations.Schema;

namespace KanbanFlow.Core
{
    public class Project
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public byte[] RowVersion { get; set; } = [];

        public virtual ICollection<Column> Columns { get; set; }

        public Project()
        {
            Columns = new HashSet<Column>();
        }

    }
}