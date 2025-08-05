namespace KanbanFlow.API.Dtos;

public record AuthResponseDto(
    string Token,
    string Username,
    string Email,
    DateTime ExpiresAt
); 