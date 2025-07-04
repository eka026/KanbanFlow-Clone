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
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TasksController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAllTasksAsync()
        {
            var tasks = await _context.TaskItems.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }

        [HttpGet("{id}", Name = "GetTaskById")]
        public async Task<ActionResult<TaskItemDto>> GetTaskByIdAsync(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TaskItemDto>(task));
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTaskAsync(CreateTaskItemDto taskDto)
        {
            var task = _mapper.Map<TaskItem>(taskDto);
            task.CreatedDate = DateTime.UtcNow;
            task.Status = Core.TaskStatus.ToDo;

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskById", new { id = task.Id }, _mapper.Map<TaskItemDto>(task));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskAsync(int id, UpdateTaskItemDto taskDto)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _mapper.Map(taskDto, task);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TaskItems.Any(e => e.Id == id))
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
        public async Task<IActionResult> DeleteTaskAsync(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
