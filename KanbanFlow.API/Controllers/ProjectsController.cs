using AutoMapper;
using KanbanFlow.API.Dtos;
using KanbanFlow.Core;
using KanbanFlow.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _unitOfWork.Projects.GetAllProjectsWithDetailsAsync();
            return Ok(_mapper.Map<IEnumerable<ProjectDto>>(projects));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var project = await _unitOfWork.Projects.GetProjectWithDetailsAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpGet("{projectId}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasksForProject(int projectId)
        {
            var projectExists = await _unitOfWork.Projects.GetByIdAsync(projectId);
            if (projectExists == null)
            {
                return NotFound("Project not found.");
            }

            var tasks = await _unitOfWork.TaskItems.FindAsync(t => t.Column != null && t.Column.ProjectId == projectId);

            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> PostProject(CreateProjectDto projectDto)
        {
            var project = _mapper.Map<Project>(projectDto);
            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, _mapper.Map<ProjectDto>(project));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, UpdateProjectDto projectDto)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
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
            var project = await _unitOfWork.Projects.GetByIdAsync(id);
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
