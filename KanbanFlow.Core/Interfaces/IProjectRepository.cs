
using KanbanFlow.Core;

namespace KanbanFlow.Core.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project?> GetProjectWithDetailsAsync(int id, int? userId = null);
        Task<IEnumerable<Project>> GetAllProjectsWithDetailsAsync(int? userId = null);
        Task<Project?> GetProjectByIdForUserAsync(int id, int? userId = null);
    }
}
