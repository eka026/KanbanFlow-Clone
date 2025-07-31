using AutoMapper;
using KanbanFlow.API.Data;
using KanbanFlow.API.Dtos;
using KanbanFlow.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProjectsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _context.Projects
                .Include(p => p.Columns)
                .ThenInclude(c => c.TaskItems)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ProjectDto>>(projects));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Columns)
                .ThenInclude(c => c.TaskItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpGet("{projectId}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasksForProject(int projectId)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
            {
                return NotFound("Project not found.");
            }

            var tasks = await _context.TaskItems
                .Where(t => t.Column != null && t.Column.ProjectId == projectId)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> PostProject(CreateProjectDto projectDto)
        {
            var project = _mapper.Map<Project>(projectDto);
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, _mapper.Map<ProjectDto>(project));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, UpdateProjectDto projectDto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _mapper.Map(projectDto, project);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Projects.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
