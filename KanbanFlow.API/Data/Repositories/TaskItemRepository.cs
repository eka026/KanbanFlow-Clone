
using KanbanFlow.Core;
using KanbanFlow.Core.Interfaces;

namespace KanbanFlow.API.Data.Repositories
{
    public class TaskItemRepository : Repository<TaskItem>, ITaskItemRepository
    {
        public TaskItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
