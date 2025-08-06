using AutoMapper;
using KanbanFlow.API.Dtos;
using KanbanFlow.Core;
using KanbanFlow.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TaskService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TaskItemDto>> GetTasksForUserAsync(int userId)
    {
        var tasks = await _unitOfWork.TaskItems.GetTasksForUserAsync(userId);
        return _mapper.Map<IEnumerable<TaskItemDto>>(tasks);
    }

    public async Task<TaskItemDto?> GetTaskByIdAsync(int id, int userId)
    {
        var task = await _unitOfWork.TaskItems.GetTaskByIdForUserAsync(id, userId);
        return task != null ? _mapper.Map<TaskItemDto>(task) : null;
    }

    public async Task<TaskItemDto> CreateTaskAsync(CreateTaskItemDto taskDto, int userId)
    {
        var column = await _unitOfWork.Columns.GetColumnWithDetailsAsync(taskDto.ColumnId, userId);
        if (column == null)
        {
            throw new InvalidOperationException("Invalid column ID.");
        }

        if (column.WipLimit.HasValue && column.TaskItems.Count >= column.WipLimit.Value)
        {
            throw new InvalidOperationException("WIP limit reached for this column.");
        }

        var task = _mapper.Map<TaskItem>(taskDto);
        task.UserId = userId;
        task.Position = column.TaskItems.Any() ? column.TaskItems.Max(t => t.Position) + 1 : 0;

        await _unitOfWork.TaskItems.AddAsync(task);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<TaskItemDto>(task);
    }

    public async Task<TaskItemDto?> UpdateTaskAsync(int id, UpdateTaskItemDto taskDto, int userId)
    {
        var task = await _unitOfWork.TaskItems.GetTaskByIdForUserAsync(id, userId);
        if (task == null)
        {
            return null;
        }

        var originalColumnId = task.ColumnId;
        var newColumnId = taskDto.ColumnId;

        if (originalColumnId != newColumnId)
        {
            var newColumn = await _unitOfWork.Columns.GetColumnWithDetailsAsync(newColumnId, userId);
            if (newColumn == null)
            {
                throw new InvalidOperationException("Invalid column ID.");
            }

            if (newColumn.WipLimit.HasValue && newColumn.TaskItems.Count >= newColumn.WipLimit.Value)
            {
                throw new InvalidOperationException("WIP limit reached for this column.");
            }

            var tasksToUpdate = await _unitOfWork.TaskItems.FindAsync(t => t.ColumnId == originalColumnId && t.Position > task.Position && t.UserId == userId);

            foreach (var taskToUpdate in tasksToUpdate)
            {
                taskToUpdate.Position--;
            }
        }

        _mapper.Map(taskDto, task);

        if (!task.ChangeStatus(taskDto.Status))
        {
            throw new InvalidOperationException("Invalid status transition.");
        }

        _unitOfWork.TaskItems.Update(task);

        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("The task has been modified by another user. Please reload and try again.");
        }

        return _mapper.Map<TaskItemDto>(task);
    }

    public async Task<bool> DeleteTaskAsync(int id, int userId)
    {
        var task = await _unitOfWork.TaskItems.GetTaskByIdForUserAsync(id, userId);
        if (task == null)
        {
            return false;
        }

        var tasksToUpdate = await _unitOfWork.TaskItems.FindAsync(t => t.ColumnId == task.ColumnId && t.Position > task.Position && t.UserId == userId);

        foreach (var taskToUpdate in tasksToUpdate)
        {
            taskToUpdate.Position--;
        }

        _unitOfWork.TaskItems.Remove(task);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> ReorderTaskAsync(int id, int newPosition, int userId)
    {
        var task = await _unitOfWork.TaskItems.GetTaskByIdForUserAsync(id, userId);
        if (task == null)
        {
            return false;
        }

        var currentPosition = task.Position;
        var columnId = task.ColumnId;

        if (currentPosition == newPosition)
        {
            return true;
        }

        var tasksInColumn = await _unitOfWork.TaskItems.GetTasksForColumnAsync(columnId, userId);
        var taskList = tasksInColumn.ToList();

        if (newPosition < 0 || newPosition >= taskList.Count)
        {
            return false;
        }

        taskList.RemoveAt(currentPosition);
        taskList.Insert(newPosition, task);

        for (int i = 0; i < taskList.Count; i++)
        {
            taskList[i].Position = i;
        }

        await _unitOfWork.CompleteAsync();
        return true;
    }
}
