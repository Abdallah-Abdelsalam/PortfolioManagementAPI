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
public class PortfoliosController : ControllerBase
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IMapper _mapper;

    public PortfoliosController(IPortfolioRepository portfolioRepository, IMapper mapper)
    {
        _portfolioRepository = portfolioRepository;
        _mapper = mapper;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    [HttpGet("my-portfolios")]
    public async Task<ActionResult<IEnumerable<PortfolioResponse>>> GetMyPortfolios()
    {
        var userId = GetCurrentUserId();
        var portfolios = await _portfolioRepository.GetPortfoliosByUserIdAsync(userId);
        var portfolioResponses = _mapper.Map<List<PortfolioResponse>>(portfolios);
        return Ok(portfolioResponses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PortfolioResponse>> GetPortfolio(int id)
    {
        var portfolio = await _portfolioRepository.GetPortfolioWithProjectsAsync(id);
        
        if (portfolio == null)
            return NotFound(new { message = $"Portfolio with ID {id} not found" });
        
        var portfolioResponse = _mapper.Map<PortfolioResponse>(portfolio);
        return Ok(portfolioResponse);
    }

    [HttpPost]
    public async Task<ActionResult<PortfolioResponse>> CreatePortfolio([FromBody] CreatePortfolioRequest request)
    {
        var userId = GetCurrentUserId();
        
        var portfolio = _mapper.Map<Portfolio>(request);
        portfolio.UserId = userId;
        portfolio.CreatedAt = DateTime.UtcNow;
        
        var createdPortfolio = await _portfolioRepository.AddAsync(portfolio);
        var portfolioResponse = _mapper.Map<PortfolioResponse>(createdPortfolio);
        
        return CreatedAtAction(nameof(GetPortfolio), new { id = createdPortfolio.Id }, portfolioResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePortfolio(int id, [FromBody] UpdatePortfolioRequest request)
    {
        var portfolio = await _portfolioRepository.GetByIdAsync(id);
        
        if (portfolio == null)
            return NotFound(new { message = $"Portfolio with ID {id} not found" });
        
        var userId = GetCurrentUserId();
        if (portfolio.UserId != userId)
            return Forbid();
        
        _mapper.Map(request, portfolio);
        portfolio.UpdatedAt = DateTime.UtcNow;
        
        await _portfolioRepository.UpdateAsync(portfolio);
        
        return Ok(new { message = "Portfolio updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePortfolio(int id)
    {
        var portfolio = await _portfolioRepository.GetByIdAsync(id);
        
        if (portfolio == null)
            return NotFound(new { message = $"Portfolio with ID {id} not found" });
        
        var userId = GetCurrentUserId();
        if (portfolio.UserId != userId)
            return Forbid();
        
        portfolio.IsActive = false;
        portfolio.UpdatedAt = DateTime.UtcNow;
        await _portfolioRepository.UpdateAsync(portfolio);
        
        return Ok(new { message = "Portfolio deleted successfully" });
    }
}
