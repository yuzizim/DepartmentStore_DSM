using AutoMapper;
using DepartmentStore.DataAccess;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.Entities;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DepartmentStore.Service.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public ProductService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventory)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var p = await _db.Products
                .Include(x => x.Category)
                .Include(x => x.Supplier)
                .Include(x => x.Inventory)
                .FirstOrDefaultAsync(x => x.Id == id);
            return p == null ? null : _mapper.Map<ProductDto>(p);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            // ensure category & supplier exist
            var cat = await _db.Categories.FindAsync(dto.CategoryId) ?? throw new KeyNotFoundException("Category not found");
            var sup = await _db.Suppliers.FindAsync(dto.SupplierId) ?? throw new KeyNotFoundException("Supplier not found");

            var entity = _mapper.Map<Product>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;

            _db.Products.Add(entity);

            // create inventory
            var inv = new Inventory
            {
                Id = Guid.NewGuid(),
                ProductId = entity.Id,
                QuantityOnHand = dto.InitialQuantity,
                ReorderLevel = 10,
                LastRestockDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _db.Inventories.Add(inv);

            await _db.SaveChangesAsync();

            // reload with nav props
            var created = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.Inventory)
                                           .FirstOrDefaultAsync(p => p.Id == entity.Id);
            return _mapper.Map<ProductDto>(created!);
        }

        public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto)
        {
            var existing = await _db.Products.FindAsync(id) ?? throw new KeyNotFoundException("Product not found");

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;
            _db.Products.Update(existing);
            await _db.SaveChangesAsync();

            var updated = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.Inventory)
                                           .FirstOrDefaultAsync(p => p.Id == id);

            return _mapper.Map<ProductDto>(updated!);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _db.Products.FindAsync(id);
            if (existing == null) return false;

            // optionally check constraints: if has order details -> forbid? For now restrict delete
            _db.Products.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
