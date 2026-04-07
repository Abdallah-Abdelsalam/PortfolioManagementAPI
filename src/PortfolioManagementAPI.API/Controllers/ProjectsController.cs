using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using PortfolioManagementAPI.API.DTOs;
using PortfolioManagementAPI.Core.Interfaces;
using PortfolioManagementAPI.Core.Entities;
using System.Security.Claims;

namespace PortfolioManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IMapper _mapper;

    public ProjectsController(IProjectRepository projectRepository, IPortfolioRepository portfolioRepository, IMapper mapper)
    {
        _projectRepository = projectRepository;
        _portfolioRepository = portfolioRepository;
        _mapper = mapper;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<ProjectResponse>>> GetProjects([FromQuery] PaginationParams paginationParams)
    {
        var projects = await _projectRepository.GetAllAsync();
        var projectResponses = _mapper.Map<List<ProjectResponse>>(projects);
        
        var query = projectResponses.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(paginationParams.SearchTerm))
        {
            query = query.Where(p => p.Name.Contains(paginationParams.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                      p.Description.Contains(paginationParams.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }
        
        var totalCount = query.Count();
        var items = query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                         .Take(paginationParams.PageSize)
                         .ToList();
        
        var response = new PagedResponse<ProjectResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize
        };
        
        return Ok(response);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProjectResponse>> GetProject(int id)
    {
        var project = await _projectRepository.GetProjectWithDetailsAsync(id);
        
        if (project == null)
            return NotFound(new { message = $"Project with ID {id} not found" });
        
        var userId = GetCurrentUserId();
        var projectResponse = _mapper.Map<ProjectResponse>(project);
        projectResponse.IsLikedByCurrentUser = await _projectRepository.IsLikedByUserAsync(id, userId);
        
        return Ok(projectResponse);
    }

    [HttpPost("portfolio/{portfolioId}")]
    public async Task<ActionResult<ProjectResponse>> CreateProject(int portfolioId, [FromBody] CreateProjectRequest request)
    {
        var portfolio = await _portfolioRepository.GetByIdAsync(portfolioId);
        
        if (portfolio == null)
            return NotFound(new { message = $"Portfolio with ID {portfolioId} not found" });
        
        var userId = GetCurrentUserId();
        if (portfolio.UserId != userId)
            return Forbid();
        
        var project = _mapper.Map<Project>(request);
        project.PortfolioId = portfolioId;
        project.CreatedAt = DateTime.UtcNow;
        
        var createdProject = await _projectRepository.AddAsync(project);
        var projectResponse = _mapper.Map<ProjectResponse>(createdProject);
        
        return CreatedAtAction(nameof(GetProject), new { id = createdProject.Id }, projectResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        
        if (project == null)
            return NotFound(new { message = $"Project with ID {id} not found" });
        
        var portfolio = await _portfolioRepository.GetByIdAsync(project.PortfolioId);
        var userId = GetCurrentUserId();
        
        if (portfolio == null || portfolio.UserId != userId)
            return Forbid();
        
        _mapper.Map(request, project);
        project.UpdatedAt = DateTime.UtcNow;
        
        await _projectRepository.UpdateAsync(project);
        
        return Ok(new { message = "Project updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        
        if (project == null)
            return NotFound(new { message = $"Project with ID {id} not found" });
        
        var portfolio = await _portfolioRepository.GetByIdAsync(project.PortfolioId);
        var userId = GetCurrentUserId();
        
        if (portfolio == null || portfolio.UserId != userId)
            return Forbid();
        
        project.IsActive = false;
        project.UpdatedAt = DateTime.UtcNow;
        await _projectRepository.UpdateAsync(project);
        
        return Ok(new { message = "Project deleted successfully" });
    }

    [HttpPost("{id}/like")]
    public async Task<IActionResult> ToggleLike(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        
        if (project == null)
            return NotFound(new { message = $"Project with ID {id} not found" });
        
        var userId = GetCurrentUserId();
        var isLiked = await _projectRepository.ToggleLikeAsync(id, userId);
        
        var message = isLiked ? "Project liked" : "Project unliked";
        return Ok(new { message, liked = isLiked });
    }
}
