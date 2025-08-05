using System.ComponentModel.DataAnnotations;

namespace KanbanFlow.Client.Dtos;

public record LoginDto(
    [Required] string Username,
    [Required] string Password
); 