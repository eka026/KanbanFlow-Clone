
using KanbanFlow.Core.Interfaces;

namespace KanbanFlow.API.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IProjectRepository Projects { get; private set; }
        public IColumnRepository Columns { get; private set; }
        public ITaskItemRepository TaskItems { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Projects = new ProjectRepository(_context);
            Columns = new ColumnRepository(_context);
            TaskItems = new TaskItemRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
