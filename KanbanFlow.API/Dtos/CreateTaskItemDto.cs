namespace KanbanFlow.API.Dtos;

public record CreateTaskItemDto(string Title, string? Description, int ColumnId);
