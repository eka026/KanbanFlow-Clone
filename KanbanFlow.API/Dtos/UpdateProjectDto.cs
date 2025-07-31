using System.ComponentModel.DataAnnotations;

namespace KanbanFlow.API.Dtos;

public record UpdateProjectDto([Required] string Name, byte[] RowVersion);
