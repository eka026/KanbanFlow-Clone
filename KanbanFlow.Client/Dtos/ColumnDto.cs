namespace KanbanFlow.Client.Dtos;

public record ColumnDto(int Id, string Name, List<TaskItemDto> TaskItems);
