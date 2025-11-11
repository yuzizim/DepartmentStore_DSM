using AutoMapper;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.DataAccess.Repositories;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DepartmentStore.Service.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IBaseRepository<Inventory> _repo;
        private readonly IMapper _mapper;

        public InventoryService(IBaseRepository<Inventory> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<InventoryDto?> GetByProductIdAsync(Guid productId)
        {
            var inv = await _repo.FindAsync(i => i.ProductId == productId)
                .ContinueWith(t => t.Result.FirstOrDefault());
            return inv == null ? null : _mapper.Map<InventoryDto>(inv);
        }

        public async Task<InventoryDto> UpdateQuantityAsync(Guid productId, UpdateInventoryDto dto, string performedBy)
        {
            var inv = await _repo.FindAsync(i => i.ProductId == productId)
                .ContinueWith(t => t.Result.FirstOrDefault())
                ?? throw new KeyNotFoundException("Inventory not found for product");

            var newQty = inv.QuantityOnHand + dto.QuantityChange;
            if (newQty < 0)
                throw new InvalidOperationException("Quantity cannot be negative");

            inv.QuantityOnHand = newQty;
            if (dto.RestockDate.HasValue)
                inv.LastRestockDate = dto.RestockDate.Value;

            inv.UpdatedAt = DateTime.UtcNow;

            _repo.Update(inv);
            await _repo.SaveChangesAsync();

            return _mapper.Map<InventoryDto>(inv);
        }
    }
}