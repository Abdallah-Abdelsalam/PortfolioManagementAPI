using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using PortfolioManagementAPI.API.DTOs;
using PortfolioManagementAPI.Core.Interfaces;

namespace PortfolioManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public AuthController(IAuthService authService, IUserRepository userRepository, IMapper mapper)
    {
        _authService = authService;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(
            request.Username,
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        if (!result)
            return BadRequest(new { message = "Username or email already exists" });

        var user = await _userRepository.GetByUsernameAsync(request.Username);
        var userResponse = _mapper.Map<UserResponse>(user);

        return Ok(new { message = "Registration successful", user = userResponse });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authService.AuthenticateAsync(request.Username, request.Password);

        if (user == null)
            return Unauthorized(new { message = "Invalid username or password" });

        var token = await _authService.GenerateJwtTokenAsync(user);
        var refreshToken = await _authService.GenerateRefreshTokenAsync();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user);

        var userResponse = _mapper.Map<UserResponse>(user);

        return Ok(new AuthResponse
        {
            Token = token,
            User = userResponse
        });
    }
}