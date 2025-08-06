
using KanbanFlow.Core.Boards;
using KanbanFlow.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Data.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Project?> GetProjectWithDetailsAsync(int id, int? userId = null)
        {
            var query = _context.Projects
                .Include(p => p.Columns)
                .ThenInclude(c => c.TaskItems)
                .Where(p => p.Id == id);
            
            if (userId.HasValue)
                query = query.Where(p => p.UserId == userId.Value);
            
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Project>> GetAllProjectsWithDetailsAsync(int? userId = null)
        {
            IQueryable<Project> query = _context.Projects
                .Include(p => p.Columns)
                .ThenInclude(c => c.TaskItems);
            
            if (userId.HasValue)
                query = query.Where(p => p.UserId == userId.Value);
            
            return await query.ToListAsync();
        }

        public async Task<Project?> GetProjectByIdForUserAsync(int id, int? userId = null)
        {
            var query = _context.Projects.Where(p => p.Id == id);
            
            if (userId.HasValue)
                query = query.Where(p => p.UserId == userId.Value);
            
            return await query.FirstOrDefaultAsync();
        }
    }
}
