using System.ComponentModel.DataAnnotations;
using KanbanFlow.Core;

namespace KanbanFlow.API.Dtos;

public record UpdateTaskItemDto([Required] string Title, string? Description, KanbanFlow.Core.TaskStatus Status, [Range(1, int.MaxValue)] int ColumnId);
