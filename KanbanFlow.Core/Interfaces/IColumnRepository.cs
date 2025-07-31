
using KanbanFlow.Core;

namespace KanbanFlow.Core.Interfaces
{
    public interface IColumnRepository : IRepository<Column>
    {
        Task<Column?> GetColumnWithDetailsAsync(int id);
    }
}
