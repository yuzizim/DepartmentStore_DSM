using AutoMapper;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.DataAccess.Repositories;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DepartmentStore.Service.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IBaseRepository<Payment> _repo;
        private readonly IBaseRepository<Order> _orderRepo;
        private readonly IMapper _mapper;

        public PaymentService(
            IBaseRepository<Payment> repo,
            IBaseRepository<Order> orderRepo,
            IMapper mapper)
        {
            _repo = repo;
            _orderRepo = orderRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _repo.GetAllWithIncludeAsync(p => p.Order!);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto?> GetByIdAsync(Guid id)
        {
            var payment = await _repo.GetByIdWithIncludeAsync(id, p => p.Order!);
            return payment == null ? null : _mapper.Map<PaymentDto>(payment);
        }

        public async Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(Guid orderId)
        {
            var payments = await _repo.FindWithIncludeAsync(
                p => p.OrderId == orderId,
                p => p.Order!
            );
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto> CreateAsync(PaymentDto dto)
        {
            var order = await _orderRepo.GetByIdAsync(dto.OrderId)
                ?? throw new KeyNotFoundException("Order not found");

            // Kiểm tra số tiền thanh toán
            if (dto.Amount != order.TotalAmount)
                throw new InvalidOperationException($"Payment amount must equal order total: {order.TotalAmount}");

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

            await _repo.AddAsync(payment);
            await _repo.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<bool> UpdateStatusAsync(Guid id, string newStatus)
        {
            var payment = await _repo.GetByIdAsync(id);
            if (payment == null) return false;

            payment.Status = newStatus;
            payment.UpdatedAt = DateTime.UtcNow;

            _repo.Update(payment);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}