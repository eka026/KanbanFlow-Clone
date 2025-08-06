using KanbanFlow.API.Dtos;

namespace KanbanFlow.API.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetProjectsForUserAsync(int userId);
    Task<ProjectDto?> GetProjectByIdAsync(int id, int userId);
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto projectDto, int userId);
    Task<ProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto projectDto, int userId);
    Task<bool> DeleteProjectAsync(int id, int userId);
}
