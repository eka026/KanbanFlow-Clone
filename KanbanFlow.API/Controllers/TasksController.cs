using AutoMapper;
using KanbanFlow.API.Dtos;
using KanbanFlow.Core;
using KanbanFlow.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TasksController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAllTasksAsync()
        {
            var tasks = await _unitOfWork.TaskItems.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }

        [HttpGet("{id}", Name = "GetTaskById")]
        public async Task<ActionResult<TaskItemDto>> GetTaskByIdAsync(int id)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TaskItemDto>(task));
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTaskAsync(CreateTaskItemDto taskDto)
        {
            var column = await _unitOfWork.Columns.GetColumnWithDetailsAsync(taskDto.ColumnId);
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

            await _unitOfWork.TaskItems.AddAsync(task);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetTaskByIdAsync), new { id = task.Id }, _mapper.Map<TaskItemDto>(task));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskAsync(int id, UpdateTaskItemDto taskDto)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var originalColumnId = task.ColumnId;
            var newColumnId = taskDto.ColumnId;

            if (originalColumnId != newColumnId)
            {
                var newColumn = await _unitOfWork.Columns.GetColumnWithDetailsAsync(newColumnId);
                if (newColumn == null)
                {
                    return BadRequest("Invalid column ID.");
                }

                if (newColumn.WipLimit.HasValue && newColumn.TaskItems.Count >= newColumn.WipLimit.Value)
                {
                    return BadRequest("WIP limit reached for this column.");
                }

                var tasksToUpdate = await _unitOfWork.TaskItems.FindAsync(t => t.ColumnId == originalColumnId && t.Position > task.Position);

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

            _unitOfWork.TaskItems.Update(task);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("The task has been modified by another user. Please reload and try again.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskAsync(int id)
        {
            var task = await _unitOfWork.TaskItems.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _unitOfWork.TaskItems.Remove(task);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpPatch("{id}/reorder")]
        public async Task<IActionResult> ReorderTaskAsync(int id, ReorderTaskDto reorderTaskDto)
        {
            var taskToMove = await _unitOfWork.TaskItems.GetByIdAsync(id);
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

            var tasksInColumn = (await _unitOfWork.TaskItems.FindAsync(t => t.ColumnId == taskToMove.ColumnId && t.Id != id)).OrderBy(t => t.Position).ToList();

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

            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
