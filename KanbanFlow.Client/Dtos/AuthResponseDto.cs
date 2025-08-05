namespace KanbanFlow.Client.Dtos;

public record AuthResponseDto(
    string Token,
    string Username,
    string Email,
    DateTime ExpiresAt
); 