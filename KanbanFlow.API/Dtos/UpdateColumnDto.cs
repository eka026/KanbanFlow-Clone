using System.ComponentModel.DataAnnotations;

namespace KanbanFlow.API.Dtos;

public record UpdateColumnDto([Required] string Name);
