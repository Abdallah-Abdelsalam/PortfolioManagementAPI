namespace PortfolioManagementAPI.Core.Entities;

public class Portfolio : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}