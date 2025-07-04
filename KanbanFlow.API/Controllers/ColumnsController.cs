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
    public class ColumnsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ColumnsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColumnDto>>> GetColumns()
        {
            var columns = await _context.Columns.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ColumnDto>>(columns));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ColumnDto>> GetColumn(int id)
        {
            var column = await _context.Columns.Include(c => c.TaskItems).FirstOrDefaultAsync(c => c.Id == id);

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
            _context.Columns.Add(column);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetColumn", new { id = column.Id }, _mapper.Map<ColumnDto>(column));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutColumn(int id, UpdateColumnDto columnDto)
        {
            var column = await _context.Columns.FindAsync(id);
            if (column == null)
            {
                return NotFound();
            }

            _mapper.Map(columnDto, column);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ColumnExists(id))
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
        public async Task<IActionResult> DeleteColumn(int id)
        {
            var column = await _context.Columns.FindAsync(id);
            if (column == null)
            {
                return NotFound();
            }

            _context.Columns.Remove(column);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ColumnExists(int id)
        {
            return _context.Columns.Any(e => e.Id == id);
        }
    }
}
