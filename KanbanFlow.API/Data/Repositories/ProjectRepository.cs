
using KanbanFlow.Core;
using KanbanFlow.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Data.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Project?> GetProjectWithDetailsAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Columns)
                .ThenInclude(c => c.TaskItems)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> GetAllProjectsWithDetailsAsync()
        {
            return await _context.Projects
                .Include(p => p.Columns)
                .ThenInclude(c => c.TaskItems)
                .ToListAsync();
        }
    }
}
