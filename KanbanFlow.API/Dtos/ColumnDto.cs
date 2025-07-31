namespace KanbanFlow.API.Dtos;

public record ColumnDto(int Id, string Name, List<TaskItemDto> TaskItems, byte[] RowVersion);
