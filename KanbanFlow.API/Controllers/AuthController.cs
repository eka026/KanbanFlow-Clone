using Microsoft.AspNetCore.Mvc;
using KanbanFlow.API.Dtos;
using KanbanFlow.Core.Common;
using KanbanFlow.API.Services;
using KanbanFlow.Core.Users;
using Microsoft.AspNetCore.Authorization;

namespace KanbanFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;

    public AuthController(IUnitOfWork unitOfWork, IJwtService jwtService, IPasswordService passwordService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _passwordService = passwordService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        // Check if username already exists
        if (await _unitOfWork.Users.UsernameExistsAsync(registerDto.Username))
        {
            return BadRequest(new { message = "Username already exists" });
        }

        // Check if email already exists
        if (await _unitOfWork.Users.EmailExistsAsync(registerDto.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        // Create new user
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = _passwordService.HashPassword(registerDto.Password)
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(Convert.ToInt32(HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:ExpiryInMinutes"] ?? "60"));

        return Ok(new AuthResponseDto(token, user.Username, user.Email ?? string.Empty, expiresAt));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        // Find user by username
        var user = await _unitOfWork.Users.GetByUsernameAsync(loginDto.Username);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Verify password
        if (!_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Update last login date
        user.LastLoginDate = DateTime.UtcNow;
        await _unitOfWork.CompleteAsync();

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(Convert.ToInt32(HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:ExpiryInMinutes"] ?? "60"));

        return Ok(new AuthResponseDto(token, user.Username, user.Email ?? string.Empty, expiresAt));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<object>> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null || !int.TryParse(userId, out var id))
        {
            return Unauthorized();
        }

        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedDate = user.CreatedDate,
            LastLoginDate = user.LastLoginDate
        });
    }
} 