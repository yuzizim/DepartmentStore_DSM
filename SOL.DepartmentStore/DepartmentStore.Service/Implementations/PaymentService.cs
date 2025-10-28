using AutoMapper;
using DepartmentStore.DataAccess;
using DepartmentStore.Entities;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DepartmentStore.Service.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public PaymentService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var list = await _db.Payments
                .Include(p => p.Order)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<PaymentDto>>(list);
        }

        public async Task<PaymentDto?> GetByIdAsync(Guid id)
        {
            var p = await _db.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(x => x.Id == id);
            return p == null ? null : _mapper.Map<PaymentDto>(p);
        }

        public async Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(Guid orderId)
        {
            var list = await _db.Payments
                .Where(p => p.OrderId == orderId)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<PaymentDto>>(list);
        }

        public async Task<PaymentDto> CreateAsync(PaymentDto dto)
        {
            var order = await _db.Orders.FindAsync(dto.OrderId)
                ?? throw new KeyNotFoundException("Order not found");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = dto.OrderId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod ?? "Cash",
                PaymentDate = DateTime.UtcNow,
                Status = "Completed",
                CreatedAt = DateTime.UtcNow
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<bool> UpdateStatusAsync(Guid id, string newStatus)
        {
            var payment = await _db.Payments.FindAsync(id);
            if (payment == null) return false;

            payment.Status = newStatus;
            payment.UpdatedAt = DateTime.UtcNow;
            _db.Payments.Update(payment);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
