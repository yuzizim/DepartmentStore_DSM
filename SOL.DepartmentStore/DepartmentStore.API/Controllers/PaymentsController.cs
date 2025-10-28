using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DepartmentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _svc;

        public PaymentsController(IPaymentService svc)
        {
            _svc = svc;
        }

        // GET: api/payments
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _svc.GetAllAsync();
            return Ok(list);
        }

        // GET: api/payments/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin,Manager,SalesEmployee")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _svc.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // GET: api/payments/order/{orderId}
        [HttpGet("order/{orderId:guid}")]
        [Authorize(Roles = "Admin,Manager,SalesEmployee")]
        public async Task<IActionResult> GetByOrder(Guid orderId)
        {
            var list = await _svc.GetByOrderIdAsync(orderId);
            return Ok(list);
        }

        // POST: api/payments
        [HttpPost]
        [Authorize(Roles = "SalesEmployee")]
        public async Task<IActionResult> Create([FromBody] PaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _svc.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PUT: api/payments/{id}/status
        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] string newStatus)
        {
            var success = await _svc.UpdateStatusAsync(id, newStatus);
            if (!success) return NotFound();
            return Ok(new { message = "Payment status updated successfully" });
        }
    }
}
