using System.ComponentModel.DataAnnotations;
using KanbanFlow.Core.Boards;
using KanbanFlow.Core.Tasks;
using KanbanFlow.Core.Users;

namespace KanbanFlow.Core.Columns
{
    public class Column
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public int? UserId { get; set; } // Foreign Key for ownership
        public int? WipLimit { get; set; }
        public DateTime CreatedDate { get; set; }

        public byte[] RowVersion { get; set; } = [];

        public virtual Project? Project { get; set; }
        public virtual User? User { get; set; }

        public virtual ICollection<TaskItem> TaskItems { get; set; }

        public Column()
        {
            CreatedDate = DateTime.UtcNow;
            TaskItems = new HashSet<TaskItem>();
        }
    }
}
