using DepartmentStore.Utilities.DTOs;

namespace DepartmentStore.Service.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryDto?> GetByProductIdAsync(Guid productId);
        Task<InventoryDto> UpdateQuantityAsync(Guid productId, UpdateInventoryDto dto, string performedBy);
    }
}
