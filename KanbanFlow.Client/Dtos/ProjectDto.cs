namespace KanbanFlow.Client.Dtos;

public record ProjectDto(int Id, string Name, List<ColumnDto> Columns);
