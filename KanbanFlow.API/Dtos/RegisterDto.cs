using System.ComponentModel.DataAnnotations;

namespace KanbanFlow.API.Dtos;

public record RegisterDto(
    [Required] [StringLength(50, MinimumLength = 3)] string Username,
    [Required] [EmailAddress] string Email,
    [Required] [StringLength(100, MinimumLength = 6)] string Password
); 