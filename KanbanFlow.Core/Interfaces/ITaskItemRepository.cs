
using KanbanFlow.Core;

namespace KanbanFlow.Core.Interfaces
{
    public interface ITaskItemRepository : IRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetTasksForUserAsync(int? userId = null);
        Task<TaskItem?> GetTaskByIdForUserAsync(int id, int? userId = null);
        Task<IEnumerable<TaskItem>> GetTasksForColumnAsync(int columnId, int? userId = null);
    }
}
