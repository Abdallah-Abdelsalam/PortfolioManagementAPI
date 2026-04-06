using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using PortfolioManagementAPI.API.DTOs;
using PortfolioManagementAPI.Core.Interfaces;
using System.Security.Claims;

namespace PortfolioManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository _commentRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public CommentsController(ICommentRepository commentRepository, IProjectRepository projectRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet("project/{projectId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CommentResponse>>> GetCommentsByProject(int projectId)
    {
        var comments = await _commentRepository.GetCommentsByProjectIdAsync(projectId);
        var commentResponses = _mapper.Map<List<CommentResponse>>(comments);
        return Ok(commentResponses);
    }

    [HttpPost("project/{projectId}")]
    public async Task<ActionResult<CommentResponse>> CreateComment(int projectId, [FromBody] CreateCommentRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project == null)
            return NotFound(new { message = $"Project with ID {projectId} not found" });

        var userId = GetCurrentUserId();

        var comment = _mapper.Map<Comment>(request);
        comment.ProjectId = projectId;
        comment.UserId = userId;
        comment.CreatedAt = DateTime.UtcNow;

        var createdComment = await _commentRepository.AddAsync(comment);
        var commentResponse = _mapper.Map<CommentResponse>(createdComment);

        return Ok(commentResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentRequest request)
    {
        var comment = await _commentRepository.GetByIdAsync(id);

        if (comment == null)
            return NotFound(new { message = $"Comment with ID {id} not found" });

        var userId = GetCurrentUserId();
        if (comment.UserId != userId)
            return Forbid();

        _mapper.Map(request, comment);
        comment.UpdatedAt = DateTime.UtcNow;

        await _commentRepository.UpdateAsync(comment);

        return Ok(new { message = "Comment updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);

        if (comment == null)
            return NotFound(new { message = $"Comment with ID {id} not found" });

        var userId = GetCurrentUserId();
        if (comment.UserId != userId)
            return Forbid();

        comment.IsActive = false;
        comment.UpdatedAt = DateTime.UtcNow;
        await _commentRepository.UpdateAsync(comment);

        return Ok(new { message = "Comment deleted successfully" });
    }
}