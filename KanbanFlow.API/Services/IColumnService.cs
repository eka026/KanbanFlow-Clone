using KanbanFlow.API.Dtos;

namespace KanbanFlow.API.Services;

public interface IColumnService
{
    Task<IEnumerable<ColumnDto>> GetColumnsForUserAsync(int userId);
    Task<ColumnDto?> GetColumnByIdAsync(int id, int userId);
    Task<ColumnDto> CreateColumnAsync(CreateColumnDto columnDto, int userId);
    Task<ColumnDto?> UpdateColumnAsync(int id, UpdateColumnDto columnDto, int userId);
    Task<bool> DeleteColumnAsync(int id, int userId);
    Task<IEnumerable<ColumnDto>> GetColumnsForProjectAsync(int projectId, int userId);
}
