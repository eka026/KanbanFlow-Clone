using KanbanFlow.Core.Tasks;

namespace KanbanFlow.Core.Common
{
    public interface ITaskItemRepository : IRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetTasksForUserAsync(int? userId = null);
        Task<TaskItem?> GetTaskByIdForUserAsync(int id, int? userId = null);
        Task<IEnumerable<TaskItem>> GetTasksForColumnAsync(int columnId, int? userId = null);
    }
}
