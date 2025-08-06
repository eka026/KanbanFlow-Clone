using System.ComponentModel.DataAnnotations;
using KanbanFlow.Core.Tasks;

namespace KanbanFlow.API.Dtos;

public record UpdateTaskItemDto([Required] string Title, string? Description, KanbanFlow.Core.Tasks.TaskStatus Status, [Range(1, int.MaxValue)] int ColumnId, byte[] RowVersion);
