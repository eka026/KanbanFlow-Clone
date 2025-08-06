using AutoMapper;
using KanbanFlow.API.Dtos;
using KanbanFlow.Core.Columns;
using KanbanFlow.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace KanbanFlow.API.Services;

public class ColumnService : IColumnService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ColumnService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ColumnDto>> GetColumnsForUserAsync(int userId)
    {
        var columns = await _unitOfWork.Columns.GetAllAsync();
        var userColumns = columns.Where(c => c.UserId == userId);
        return _mapper.Map<IEnumerable<ColumnDto>>(userColumns);
    }

    public async Task<ColumnDto?> GetColumnByIdAsync(int id, int userId)
    {
        var column = await _unitOfWork.Columns.GetColumnWithDetailsAsync(id, userId);
        return column != null ? _mapper.Map<ColumnDto>(column) : null;
    }

    public async Task<ColumnDto> CreateColumnAsync(CreateColumnDto columnDto, int userId)
    {
        var project = await _unitOfWork.Projects.GetProjectByIdForUserAsync(columnDto.ProjectId, userId);
        if (project == null)
        {
            throw new InvalidOperationException("Invalid project ID.");
        }

        var column = _mapper.Map<Column>(columnDto);
        column.UserId = userId;

        await _unitOfWork.Columns.AddAsync(column);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<ColumnDto>(column);
    }

    public async Task<ColumnDto?> UpdateColumnAsync(int id, UpdateColumnDto columnDto, int userId)
    {
        var column = await _unitOfWork.Columns.GetColumnByIdForUserAsync(id, userId);
        if (column == null)
        {
            return null;
        }

        _mapper.Map(columnDto, column);

        try
        {
            _unitOfWork.Columns.Update(column);
            await _unitOfWork.CompleteAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("The column has been modified by another user. Please reload and try again.");
        }

        return _mapper.Map<ColumnDto>(column);
    }

    public async Task<bool> DeleteColumnAsync(int id, int userId)
    {
        var column = await _unitOfWork.Columns.GetColumnByIdForUserAsync(id, userId);
        if (column == null)
        {
            return false;
        }

        _unitOfWork.Columns.Remove(column);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<IEnumerable<ColumnDto>> GetColumnsForProjectAsync(int projectId, int userId)
    {
        var columns = await _unitOfWork.Columns.GetColumnsForProjectAsync(projectId, userId);
        return _mapper.Map<IEnumerable<ColumnDto>>(columns);
    }
}
