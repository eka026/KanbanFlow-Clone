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
            var column = await _context.Columns.Include(c => c.TaskItems).FirstOrDefaultAsync(c => c.Id == taskDto.ColumnId);
            if (column == null)
            {
                return BadRequest("Invalid column ID.");
            }

            if (column.WipLimit.HasValue && column.TaskItems.Count >= column.WipLimit.Value)
            {
                return BadRequest("WIP limit reached for this column.");
            }

            var task = _mapper.Map<TaskItem>(taskDto);
            task.CreatedDate = DateTime.UtcNow;
            task.Status = Core.TaskStatus.ToDo;
            task.Position = column.TaskItems.Any() ? column.TaskItems.Max(t => t.Position) + 1 : 0;

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

            var originalColumnId = task.ColumnId;
            var newColumnId = taskDto.ColumnId;

            if (originalColumnId != newColumnId)
            {
                var newColumn = await _context.Columns.Include(c => c.TaskItems).FirstOrDefaultAsync(c => c.Id == newColumnId);
                if (newColumn == null)
                {
                    return BadRequest("Invalid column ID.");
                }

                if (newColumn.WipLimit.HasValue && newColumn.TaskItems.Count >= newColumn.WipLimit.Value)
                {
                    return BadRequest("WIP limit reached for this column.");
                }

                // Re-organize the positions in the original column
                var tasksToUpdate = await _context.TaskItems
                    .Where(t => t.ColumnId == originalColumnId && t.Position > task.Position)
                    .ToListAsync();

                foreach (var taskToUpdate in tasksToUpdate)
                {
                    taskToUpdate.Position--;
                }
            }

            _mapper.Map(taskDto, task);

            if (!task.ChangeStatus(taskDto.Status))
            {
                return BadRequest("Invalid status transition.");
            }

            _context.Entry(task).Property(p => p.RowVersion).OriginalValue = taskDto.RowVersion;

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
                    return Conflict("The task has been modified by another user. Please reload and try again.");
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

        [HttpPatch("{id}/reorder")]
        public async Task<IActionResult> ReorderTaskAsync(int id, ReorderTaskDto reorderTaskDto)
        {
            var taskToMove = await _context.TaskItems.FindAsync(id);
            if (taskToMove == null)
            {
                return NotFound();
            }

            var oldPosition = taskToMove.Position;
            var newPosition = reorderTaskDto.Position;

            if (oldPosition == newPosition)
            {
                return NoContent(); // Nothing to do
            }

            var tasksInColumn = await _context.TaskItems
                .Where(t => t.ColumnId == taskToMove.ColumnId && t.Id != id)
                .OrderBy(t => t.Position)
                .ToListAsync();

            if (newPosition < oldPosition)
            {
                // Task is moving up, shift other tasks down
                foreach (var task in tasksInColumn.Where(t => t.Position >= newPosition && t.Position < oldPosition))
                {
                    task.Position++;
                }
            }
            else
            {
                // Task is moving down, shift other tasks up
                foreach (var task in tasksInColumn.Where(t => t.Position > oldPosition && t.Position <= newPosition))
                {
                    task.Position--;
                }
            }

            taskToMove.Position = newPosition;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
