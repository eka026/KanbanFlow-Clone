using KanbanFlow.Core;

namespace KanbanFlow.Client.Dtos;

public record TaskItemDto(int Id, string Title, string? Description, KanbanFlow.Core.TaskStatus Status, DateTime CreatedDate);
