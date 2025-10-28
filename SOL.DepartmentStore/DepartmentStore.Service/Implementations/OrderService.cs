using AutoMapper;
using DepartmentStore.DataAccess;
using DepartmentStore.Entities;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace DepartmentStore.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var list = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(list);
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var o = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
            return o == null ? null : _mapper.Map<OrderDto>(o);
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto, Guid employeeId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var customer = await _db.Customers.FindAsync(dto.CustomerId)
                    ?? throw new KeyNotFoundException("Customer not found");

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    EmployeeId = employeeId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Confirmed
                };
                _db.Orders.Add(order);

                decimal total = 0;
                foreach (var item in dto.OrderDetails)
                {
                    var product = await _db.Products.Include(p => p.Inventory)
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId)
                        ?? throw new KeyNotFoundException($"Product {item.ProductId} not found");

                    if (product.Inventory == null || product.Inventory.QuantityOnHand < item.Quantity)
                        throw new InvalidOperationException($"Not enough stock for {product.Name}");

                    product.Inventory.QuantityOnHand -= item.Quantity;

                    var detail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };
                    _db.OrderDetails.Add(detail);
                    total += product.Price * item.Quantity;
                }

                order.TotalAmount = total;

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Amount = total,
                    PaymentMethod = dto.PaymentMethod,
                    PaymentDate = DateTime.UtcNow,
                    Status = "Completed",
                    CreatedAt = DateTime.UtcNow
                };
                _db.Payments.Add(payment);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                var created = await _db.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails).ThenInclude(d => d.Product)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                return _mapper.Map<OrderDto>(created!);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderDto> UpdateStatusAsync(Guid id, string newStatus)
        {
            var order = await _db.Orders.FindAsync(id)
                ?? throw new KeyNotFoundException("Order not found");

            if (!Enum.TryParse<OrderStatus>(newStatus, out var status))
                throw new ArgumentException("Invalid status");

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
            return _mapper.Map<OrderDto>(order);
        }
    }
}
