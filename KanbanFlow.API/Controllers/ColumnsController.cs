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
    public class ColumnsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ColumnsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColumnDto>>> GetColumns()
        {
            var columns = await _unitOfWork.Columns.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ColumnDto>>(columns));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ColumnDto>> GetColumn(int id)
        {
            var column = await _unitOfWork.Columns.GetColumnWithDetailsAsync(id);

            if (column == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ColumnDto>(column));
        }

        [HttpPost]
        public async Task<ActionResult<ColumnDto>> PostColumn(CreateColumnDto columnDto)
        {
            var column = _mapper.Map<Column>(columnDto);
            await _unitOfWork.Columns.AddAsync(column);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction("GetColumn", new { id = column.Id }, _mapper.Map<ColumnDto>(column));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutColumn(int id, UpdateColumnDto columnDto)
        {
            var column = await _unitOfWork.Columns.GetByIdAsync(id);
            if (column == null)
            {
                return NotFound();
            }

            _mapper.Map(columnDto, column);

            _unitOfWork.Columns.Update(column);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("The column has been modified by another user. Please reload and try again.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(int id)
        {
            var column = await _unitOfWork.Columns.GetByIdAsync(id);
            if (column == null)
            {
                return NotFound();
            }

            _unitOfWork.Columns.Remove(column);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
