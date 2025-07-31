using System.ComponentModel.DataAnnotations;

namespace KanbanFlow.API.Dtos;

public record CreateTaskItemDto([Required] string Title, string? Description, [Range(1, int.MaxValue)] int ColumnId);
