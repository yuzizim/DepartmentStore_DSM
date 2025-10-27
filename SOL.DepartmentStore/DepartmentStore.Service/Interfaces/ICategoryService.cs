using DepartmentStore.Utilities.DTOs;

namespace DepartmentStore.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(Guid id);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
