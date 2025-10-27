using AutoMapper;
using DepartmentStore.DataAccess;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DepartmentStore.Service.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public InventoryService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<InventoryDto?> GetByProductIdAsync(Guid productId)
        {
            var inv = await _db.Inventories.AsNoTracking().FirstOrDefaultAsync(i => i.ProductId == productId);
            return inv == null ? null : _mapper.Map<InventoryDto>(inv);
        }

        public async Task<InventoryDto> UpdateQuantityAsync(Guid productId, UpdateInventoryDto dto, string performedBy)
        {
            var inv = await _db.Inventories.FirstOrDefaultAsync(i => i.ProductId == productId);
            if (inv == null) throw new KeyNotFoundException("Inventory not found for product");

            var newQty = inv.QuantityOnHand + dto.QuantityChange;
            if (newQty < 0)
                throw new InvalidOperationException("Quantity cannot be negative");

            inv.QuantityOnHand = newQty;
            if (dto.RestockDate.HasValue)
                inv.LastRestockDate = dto.RestockDate.Value;
            inv.UpdatedAt = DateTime.UtcNow;

            _db.Inventories.Update(inv);
            await _db.SaveChangesAsync();

            // optional: create inventory log record (not implemented)

            return _mapper.Map<InventoryDto>(inv);
        }
    }
}
