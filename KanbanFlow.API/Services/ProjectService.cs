using AutoMapper;
using KanbanFlow.API.Dtos;
using KanbanFlow.Core;
using KanbanFlow.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsForUserAsync(int userId)
    {
        var projects = await _unitOfWork.Projects.GetAllProjectsWithDetailsAsync(userId);
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(int id, int userId)
    {
        var project = await _unitOfWork.Projects.GetProjectWithDetailsAsync(id, userId);
        return project != null ? _mapper.Map<ProjectDto>(project) : null;
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto projectDto, int userId)
    {
        var project = _mapper.Map<Project>(projectDto);
        project.UserId = userId;

        await _unitOfWork.Projects.AddAsync(project);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto projectDto, int userId)
    {
        var project = await _unitOfWork.Projects.GetProjectByIdForUserAsync(id, userId);
        if (project == null)
        {
            return null;
        }

        _mapper.Map(projectDto, project);

        try
        {
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.CompleteAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("The project has been modified by another user. Please reload and try again.");
        }

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<bool> DeleteProjectAsync(int id, int userId)
    {
        var project = await _unitOfWork.Projects.GetProjectByIdForUserAsync(id, userId);
        if (project == null)
        {
            return false;
        }

        _unitOfWork.Projects.Remove(project);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}
