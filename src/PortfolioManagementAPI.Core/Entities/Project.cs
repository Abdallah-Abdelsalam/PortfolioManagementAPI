snamespace PortfolioManagementAPI.Core.Entities;

public class Project : BaseEntity
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string? ProjectUrl { get; set; }
	public string? RepositoryUrl { get; set; }
	public string? ImageUrl { get; set; }
	public int PortfolioId { get; set; }

	public virtual Portfolio Portfolio { get; set; } = null!;
	public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
	public virtual ICollection<ProjectLike> Likes { get; set; } = new List<ProjectLike>();
}