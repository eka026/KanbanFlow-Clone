
namespace KanbanFlow.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProjectRepository Projects { get; }
        IColumnRepository Columns { get; }
        ITaskItemRepository TaskItems { get; }
        IUserRepository Users { get; }
        Task<int> CompleteAsync();
    }
}
