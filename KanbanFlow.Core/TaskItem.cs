using System.Diagnostics.Contracts;

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
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public TaskStatus Status { get; set; }

        public int ColumnId { get; set; } // Foreign Key
        public int Position { get; set; }

        public virtual Column? Column { get; set; }

        public bool ChangeStatus(TaskStatus newStatus)
        {
            if (Status == newStatus) return true;

            switch (Status)
            {
                case TaskStatus.ToDo:
                    if (newStatus == TaskStatus.InProgress)
                    {
                        Status = newStatus;
                        return true;
                    }
                    break;
                case TaskStatus.InProgress:
                    if (newStatus == TaskStatus.Done || newStatus == TaskStatus.ToDo)
                    {
                        Status = newStatus;
                        return true;
                    }
                    break;
                case TaskStatus.Done:
                    // Tasks that are done cannot be moved back to other states.
                    break;
            }
            return false;
        } 
    }
}