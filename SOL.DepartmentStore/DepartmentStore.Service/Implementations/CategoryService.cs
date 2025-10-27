using AutoMapper;
using DepartmentStore.DataAccess;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.Entities;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DepartmentStore.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public CategoryService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var list = await _db.Categories.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(list);
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            var e = await _db.Categories.FindAsync(id);
            return e == null ? null : _mapper.Map<CategoryDto>(e);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var e = _mapper.Map<Category>(dto);
            e.Id = Guid.NewGuid();
            e.CreatedAt = DateTime.UtcNow;
            _db.Categories.Add(e);
            await _db.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(e);
        }

        public async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto)
        {
            var e = await _db.Categories.FindAsync(id) ?? throw new KeyNotFoundException("Category not found");
            _mapper.Map(dto, e);
            e.UpdatedAt = DateTime.UtcNow;
            _db.Categories.Update(e);
            await _db.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(e);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var e = await _db.Categories.FindAsync(id);
            if (e == null) return false;
            _db.Categories.Remove(e);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
