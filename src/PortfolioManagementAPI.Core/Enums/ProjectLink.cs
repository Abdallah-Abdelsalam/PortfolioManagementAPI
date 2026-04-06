namespace PortfolioManagementAPI.Core.Entities;

public class ProjectLike
{
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public DateTime LikedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
}