namespace KanbanFlow.API.Dtos;

public record ProjectDto(int Id, string Name, List<ColumnDto> Columns);
