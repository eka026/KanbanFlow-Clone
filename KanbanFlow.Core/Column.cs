namespace KanbanFlow.Core
{
    public class Column
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int ProjectId { get; set; }
        public int? WipLimit { get; set; }

        public byte[] RowVersion { get; set; } = [];

        public virtual Project? Project { get; set; }

        public virtual ICollection<TaskItem> TaskItems { get; set; }

        public Column()
        {
            TaskItems = new HashSet<TaskItem>();
        }
    }
}