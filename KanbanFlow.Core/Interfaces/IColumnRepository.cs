
using KanbanFlow.Core;

namespace KanbanFlow.Core.Interfaces
{
    public interface IColumnRepository : IRepository<Column>
    {
        Task<Column?> GetColumnWithDetailsAsync(int id, int? userId = null);
        Task<IEnumerable<Column>> GetColumnsForProjectAsync(int projectId, int? userId = null);
        Task<Column?> GetColumnByIdForUserAsync(int id, int? userId = null);
    }
}
