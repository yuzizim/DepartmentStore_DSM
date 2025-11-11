using AutoMapper;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.DataAccess.Repositories;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;

namespace DepartmentStore.Service.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepository<OrderDetail> _orderDetailRepo;
        private readonly IBaseRepository<Product> _productRepo;
        private readonly IBaseRepository<Inventory> _inventoryRepo;
        private readonly IBaseRepository<Category> _categoryRepo;
        private readonly IBaseRepository<Supplier> _supplierRepo;
        private readonly IMapper _mapper;

        public ProductService(
            IBaseRepository<Product> productRepo,
            IBaseRepository<Inventory> inventoryRepo,
            IBaseRepository<Category> categoryRepo,
            IBaseRepository<Supplier> supplierRepo,
            IMapper mapper)
        {
            _productRepo = productRepo;
            _inventoryRepo = inventoryRepo;
            _categoryRepo = categoryRepo;
            _supplierRepo = supplierRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepo.GetAllWithIncludeAsync(
                p => p.Category!,
                p => p.Supplier!,
                p => p.Inventory!
            );
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _productRepo.GetByIdWithIncludeAsync(id,
                p => p.Category!,
                p => p.Supplier!,
                p => p.Inventory!
            );
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId)
                ?? throw new KeyNotFoundException("Category not found");
            var supplier = await _supplierRepo.GetByIdAsync(dto.SupplierId)
                ?? throw new KeyNotFoundException("Supplier not found");

            var product = _mapper.Map<Product>(dto);
            product.Id = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            var inventory = new Inventory
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                QuantityOnHand = dto.InitialQuantity,
                ReorderLevel = 10,
                LastRestockDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            await _inventoryRepo.AddAsync(inventory);
            await _inventoryRepo.SaveChangesAsync();

            var created = await _productRepo.GetByIdWithIncludeAsync(product.Id,
                p => p.Category!,
                p => p.Supplier!,
                p => p.Inventory!
            );

            return _mapper.Map<ProductDto>(created!);
        }

        public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _productRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Product not found");

            _mapper.Map(dto, product);
            product.UpdatedAt = DateTime.UtcNow;

            _productRepo.Update(product);
            await _productRepo.SaveChangesAsync();

            var updated = await _productRepo.GetByIdWithIncludeAsync(id,
                p => p.Category!,
                p => p.Supplier!,
                p => p.Inventory!
            );

            return _mapper.Map<ProductDto>(updated!);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _productRepo.GetByIdWithIncludeAsync(id, p => p.OrderDetails!);
            if (product == null) return false;

            if (product.OrderDetails.Any())
                throw new InvalidOperationException("Cannot delete product with existing orders");

            _productRepo.Remove(product); // Soft Delete
            await _productRepo.SaveChangesAsync();
            return true;
        }
    }
}