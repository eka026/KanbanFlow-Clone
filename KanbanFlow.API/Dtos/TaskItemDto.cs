using KanbanFlow.Core.Tasks;

namespace KanbanFlow.API.Dtos;

public record TaskItemDto(int Id, string Title, string? Description, KanbanFlow.Core.Tasks.TaskStatus Status, int ColumnId, int? UserId, int Position);
