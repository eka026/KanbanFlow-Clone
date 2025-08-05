using System.ComponentModel.DataAnnotations;

namespace KanbanFlow.API.Dtos;

public record LoginDto(
    [Required] string Username,
    [Required] string Password
); 