using KanbanFlow.API.Dtos;

namespace KanbanFlow.API.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskItemDto>> GetTasksForUserAsync(int userId);
    Task<TaskItemDto?> GetTaskByIdAsync(int id, int userId);
    Task<TaskItemDto> CreateTaskAsync(CreateTaskItemDto taskDto, int userId);
    Task<TaskItemDto?> UpdateTaskAsync(int id, UpdateTaskItemDto taskDto, int userId);
    Task<bool> DeleteTaskAsync(int id, int userId);
    Task<bool> ReorderTaskAsync(int id, int newPosition, int userId);
}
