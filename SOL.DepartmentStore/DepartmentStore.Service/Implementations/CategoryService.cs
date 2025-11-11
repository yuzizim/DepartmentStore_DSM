using AutoMapper;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.DataAccess.Repositories;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DepartmentStore.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseRepository<Category> _repo;
        private readonly IMapper _mapper;

        public CategoryService(IBaseRepository<Category> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(entities);
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<CategoryDto>(entity);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var entity = _mapper.Map<Category>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto)
        {
            var entity = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Category not found");

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();

            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            _repo.Remove(entity); // ← Soft Delete (IsDeleted = true)
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}