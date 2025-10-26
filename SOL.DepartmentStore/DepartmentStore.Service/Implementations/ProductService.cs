using AutoMapper;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.DataAccess.Repositories;
using DepartmentStore.Entities;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;

namespace DepartmentStore.Service.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repo;
        private readonly IMapper _mapper;

        public ProductService(IRepository<Product> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _repo.AddAsync(product);
            await _repo.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Product not found");

            _mapper.Map(dto, existing);
            _repo.Update(existing);
            await _repo.SaveChangesAsync();
            return _mapper.Map<ProductDto>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null) return false;

            _repo.Remove(product);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
