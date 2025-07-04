using KanbanFlow.Core;

namespace KanbanFlow.Client.Dtos;

public record TaskItemDto(int Id, string Title, string? Description, TaskStatus Status, DateTime CreatedDate);
