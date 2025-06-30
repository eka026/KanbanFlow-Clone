namespace KanbanFlow.Core
{
    // Enum to represent the status of a task
    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public TaskStatus Status { get; set; }
    }
}