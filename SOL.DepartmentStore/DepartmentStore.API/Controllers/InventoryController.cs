using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DepartmentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _svc;

        public InventoryController(IInventoryService svc) => _svc = svc;

        [HttpGet("product/{productId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            var inv = await _svc.GetByProductIdAsync(productId);
            if (inv == null) return NotFound();
            return Ok(inv);
        }

        // only Manager or InventoryEmployee or Admin can change inventory
        [HttpPost("product/{productId:guid}/adjust")]
        [Authorize(Roles = "Admin,Manager,InventoryEmployee")]
        public async Task<IActionResult> AdjustQuantity(Guid productId, [FromBody] UpdateInventoryDto dto)
        {
            // you can extract performedBy from claims if needed
            var performedBy = User?.Identity?.Name ?? "system";
            try
            {
                var updated = await _svc.UpdateQuantityAsync(productId, dto, performedBy);
                return Ok(updated);
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }
    }
}
