
using KanbanFlow.Core;
using KanbanFlow.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Data.Repositories
{
    public class TaskItemRepository : Repository<TaskItem>, ITaskItemRepository
    {
        public TaskItemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaskItem>> GetTasksForUserAsync(int? userId = null)
        {
            IQueryable<TaskItem> query = _context.TaskItems
                .Include(t => t.Column!)
                .ThenInclude(c => c.Project);
            
            if (userId.HasValue)
                query = query.Where(t => t.UserId == userId.Value);
            
            return await query.ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdForUserAsync(int id, int? userId = null)
        {
            IQueryable<TaskItem> query = _context.TaskItems
                .Include(t => t.Column!)
                .ThenInclude(c => c.Project)
                .Where(t => t.Id == id);
            
            if (userId.HasValue)
                query = query.Where(t => t.UserId == userId.Value);
            
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksForColumnAsync(int columnId, int? userId = null)
        {
            IQueryable<TaskItem> query = _context.TaskItems
                .Where(t => t.ColumnId == columnId);
            
            if (userId.HasValue)
                query = query.Where(t => t.UserId == userId.Value);
            
            return await query.OrderBy(t => t.Position).ToListAsync();
        }
    }
}
