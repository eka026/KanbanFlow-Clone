
using KanbanFlow.Core.Columns;
using KanbanFlow.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Data.Repositories
{
    public class ColumnRepository : Repository<Column>, IColumnRepository
    {
        public ColumnRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Column?> GetColumnWithDetailsAsync(int id, int? userId = null)
        {
            var query = _context.Columns
                .Include(c => c.TaskItems)
                .Include(c => c.Project)
                .Where(c => c.Id == id);
            
            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);
            
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Column>> GetColumnsForProjectAsync(int projectId, int? userId = null)
        {
            var query = _context.Columns
                .Include(c => c.TaskItems)
                .Where(c => c.ProjectId == projectId);
            
            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);
            
            return await query.ToListAsync();
        }

        public async Task<Column?> GetColumnByIdForUserAsync(int id, int? userId = null)
        {
            var query = _context.Columns.Where(c => c.Id == id);
            
            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);
            
            return await query.FirstOrDefaultAsync();
        }
    }
}
