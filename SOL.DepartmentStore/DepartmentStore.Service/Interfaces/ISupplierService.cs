using DepartmentStore.Utilities.DTOs;

namespace DepartmentStore.Service.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDto>> GetAllAsync();
        Task<SupplierDto?> GetByIdAsync(Guid id);
        Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
        Task<SupplierDto> UpdateAsync(Guid id, UpdateSupplierDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
