using AutoMapper;
using PortfolioManagementAPI.API.DTOs;
using PortfolioManagementAPI.Core.Entities;

namespace PortfolioManagementAPI.API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserResponse>();
        CreateMap<RegisterRequest, User>();
        CreateMap<UpdateUserRequest, User>();

        CreateMap<Portfolio, PortfolioResponse>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.ProjectCount, opt => opt.MapFrom(src => src.Projects.Count(p => p.IsActive)));

        CreateMap<CreatePortfolioRequest, Portfolio>();
        CreateMap<UpdatePortfolioRequest, Portfolio>();

        CreateMap<Project, ProjectResponse>()
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count(c => c.IsActive)));

        CreateMap<CreateProjectRequest, Project>();
        CreateMap<UpdateProjectRequest, Project>();

        CreateMap<Comment, CommentResponse>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => src.User.AvatarUrl ?? string.Empty));

        CreateMap<CreateCommentRequest, Comment>();
        CreateMap<UpdateCommentRequest, Comment>();
    }
}