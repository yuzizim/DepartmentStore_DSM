using DepartmentStore.Utilities.DTOs;

namespace DepartmentStore.Service.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync();
        Task<PaymentDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(Guid orderId);
        Task<PaymentDto> CreateAsync(PaymentDto dto);
        Task<bool> UpdateStatusAsync(Guid id, string newStatus);
    }
}
