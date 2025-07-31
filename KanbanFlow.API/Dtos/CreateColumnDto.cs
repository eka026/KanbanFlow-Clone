using System.ComponentModel.DataAnnotations;

namespace KanbanFlow.API.Dtos;

public record CreateColumnDto([Required] string Name, [Range(1, int.MaxValue)] int ProjectId);
