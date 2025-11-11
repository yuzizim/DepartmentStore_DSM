using AutoMapper;
using DepartmentStore.DataAccess;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DepartmentStore.Service.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public SupplierService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllAsync()
        {
            var list = await _db.Suppliers.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<SupplierDto>>(list);
        }

        public async Task<SupplierDto?> GetByIdAsync(Guid id)
        {
            var e = await _db.Suppliers.FindAsync(id);
            return e == null ? null : _mapper.Map<SupplierDto>(e);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
        {
            var e = _mapper.Map<Supplier>(dto);
            e.Id = Guid.NewGuid();
            e.CreatedAt = DateTime.UtcNow;
            _db.Suppliers.Add(e);
            await _db.SaveChangesAsync();
            return _mapper.Map<SupplierDto>(e);
        }

        public async Task<SupplierDto> UpdateAsync(Guid id, UpdateSupplierDto dto)
        {
            var e = await _db.Suppliers.FindAsync(id) ?? throw new KeyNotFoundException("Supplier not found");
            _mapper.Map(dto, e);
            e.UpdatedAt = DateTime.UtcNow;
            _db.Suppliers.Update(e);
            await _db.SaveChangesAsync();
            return _mapper.Map<SupplierDto>(e);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var e = await _db.Suppliers.FindAsync(id);
            if (e == null) return false;
            _db.Suppliers.Remove(e);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
