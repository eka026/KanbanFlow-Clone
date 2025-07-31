
using KanbanFlow.Core;
using KanbanFlow.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Data.Repositories
{
    public class ColumnRepository : Repository<Column>, IColumnRepository
    {
        public ColumnRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Column?> GetColumnWithDetailsAsync(int id)
        {
            return await _context.Columns.Include(c => c.TaskItems).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
