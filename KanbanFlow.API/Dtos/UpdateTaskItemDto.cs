using KanbanFlow.Core;

namespace KanbanFlow.API.Dtos;

public record UpdateTaskItemDto(string Title, string? Description, KanbanFlow.Core.TaskStatus Status, int ColumnId);
