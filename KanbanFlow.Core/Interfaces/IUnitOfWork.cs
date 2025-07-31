
namespace KanbanFlow.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProjectRepository Projects { get; }
        IColumnRepository Columns { get; }
        ITaskItemRepository TaskItems { get; }
        Task<int> CompleteAsync();
    }
}
