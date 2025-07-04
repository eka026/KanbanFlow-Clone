using KanbanFlow.Core;

namespace KanbanFlow.API.Dtos;

public record UpdateTaskItemDto(string Title, string? Description, TaskStatus Status, int ColumnId);
