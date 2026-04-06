namespace PortfolioManagementAPI.Core.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public int UserId { get; set; }

    public virtual Project Project { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}