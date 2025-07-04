namespace KanbanFlow.Client.Dtos;

public record CreateTaskItemDto(string Title, string? Description, int ColumnId);
