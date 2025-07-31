
using KanbanFlow.Core;

namespace KanbanFlow.Core.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project?> GetProjectWithDetailsAsync(int id);
        Task<IEnumerable<Project>> GetAllProjectsWithDetailsAsync();
    }
}
