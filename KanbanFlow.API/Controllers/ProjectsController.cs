using AutoMapper;
using KanbanFlow.API.Dtos;
using KanbanFlow.Core;
using KanbanFlow.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using KanbanFlow.API.Services;

namespace KanbanFlow.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public ProjectsController(IUnitOfWork unitOfWork, IMapper mapper, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var projects = await _unitOfWork.Projects.GetAllProjectsWithDetailsAsync(userId);
            return Ok(_mapper.Map<IEnumerable<ProjectDto>>(projects));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var project = await _unitOfWork.Projects.GetProjectWithDetailsAsync(id, userId);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpGet("{projectId}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasksForProject(int projectId)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var projectExists = await _unitOfWork.Projects.GetProjectByIdForUserAsync(projectId, userId);
            if (projectExists == null)
            {
                return NotFound("Project not found.");
            }

            var tasks = await _unitOfWork.TaskItems.GetTasksForColumnAsync(projectId, userId);

            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> PostProject(CreateProjectDto projectDto)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var project = _mapper.Map<Project>(projectDto);
            project.UserId = userId;
            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, _mapper.Map<ProjectDto>(project));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, UpdateProjectDto projectDto)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var project = await _unitOfWork.Projects.GetProjectByIdForUserAsync(id, userId);
            if (project == null)
            {
                return NotFound();
            }

            _mapper.Map(projectDto, project);

            _unitOfWork.Projects.Update(project);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("The project has been modified by another user. Please reload and try again.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var project = await _unitOfWork.Projects.GetProjectByIdForUserAsync(id, userId);
            if (project == null)
            {
                return NotFound();
            }

            _unitOfWork.Projects.Remove(project);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
