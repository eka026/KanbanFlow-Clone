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
    public class ColumnsController : ControllerBase
    {
        private readonly IColumnService _columnService;
        private readonly IUserContextService _userContextService;

        public ColumnsController(IColumnService columnService, IUserContextService userContextService)
        {
            _columnService = columnService;
            _userContextService = userContextService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColumnDto>>> GetColumns()
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var columns = await _columnService.GetColumnsForUserAsync(userId.Value);
                return Ok(columns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching columns.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ColumnDto>> GetColumn(int id)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var column = await _columnService.GetColumnByIdAsync(id, userId.Value);
                if (column == null)
                {
                    return NotFound();
                }

                return Ok(column);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the column.", error = ex.Message });
            }
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<ColumnDto>>> GetColumnsForProject(int projectId)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var columns = await _columnService.GetColumnsForProjectAsync(projectId, userId.Value);
                return Ok(columns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching columns for the project.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ColumnDto>> CreateColumn(CreateColumnDto columnDto)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var createdColumn = await _columnService.CreateColumnAsync(columnDto, userId.Value);
                return CreatedAtAction(nameof(GetColumn), new { id = createdColumn.Id }, createdColumn);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the column.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateColumn(int id, UpdateColumnDto columnDto)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var updatedColumn = await _columnService.UpdateColumnAsync(id, columnDto, userId.Value);
                if (updatedColumn == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the column.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(int id)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var deleted = await _columnService.DeleteColumnAsync(id, userId.Value);
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the column.", error = ex.Message });
            }
        }
    }
}
