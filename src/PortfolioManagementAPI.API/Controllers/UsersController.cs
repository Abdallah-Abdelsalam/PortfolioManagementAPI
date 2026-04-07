using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using PortfolioManagementAPI.API.DTOs;
using PortfolioManagementAPI.Core.Interfaces;

namespace PortfolioManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PagedResponse<UserResponse>>> GetUsers([FromQuery] PaginationParams paginationParams)
    {
        var users = await _userRepository.GetAllAsync();
        var userResponses = _mapper.Map<List<UserResponse>>(users);
        
        var query = userResponses.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(paginationParams.SearchTerm))
        {
            query = query.Where(u => u.Username.Contains(paginationParams.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                      u.Email.Contains(paginationParams.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }
        
        var totalCount = query.Count();
        var items = query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                         .Take(paginationParams.PageSize)
                         .ToList();
        
        var response = new PagedResponse<UserResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        };
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return NotFound(new { message = $"User with ID {id} not found" });
        
        var userResponse = _mapper.Map<UserResponse>(user);
        return Ok(userResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return NotFound(new { message = $"User with ID {id} not found" });
        
        _mapper.Map(request, user);
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        
        return Ok(new { message = "User updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return NotFound(new { message = $"User with ID {id} not found" });
        
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        
        return Ok(new { message = "User deleted successfully" });
    }
}
