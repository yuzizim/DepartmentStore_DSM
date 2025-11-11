using AutoMapper;
using DepartmentStore.DataAccess;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.DataAccess.Repositories;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DepartmentStore.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IBaseRepository<Order> _orderRepo;
        private readonly IBaseRepository<OrderDetail> _detailRepo;
        private readonly IBaseRepository<Payment> _paymentRepo;
        private readonly IBaseRepository<Product> _productRepo;
        private readonly IBaseRepository<Inventory> _inventoryRepo;
        private readonly IBaseRepository<Customer> _customerRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public OrderService(
            IBaseRepository<Order> orderRepo,
            IBaseRepository<OrderDetail> detailRepo,
            IBaseRepository<Payment> paymentRepo,
            IBaseRepository<Product> productRepo,
            IBaseRepository<Inventory> inventoryRepo,
            IBaseRepository<Customer> customerRepo,
            IMapper mapper,
            AppDbContext db)
        {
            _orderRepo = orderRepo;
            _detailRepo = detailRepo;
            _paymentRepo = paymentRepo;
            _productRepo = productRepo;
            _inventoryRepo = inventoryRepo;
            _customerRepo = customerRepo;
            _mapper = mapper;
            _db = db;
        }
        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepo.GetAllWithIncludeAsync(
                o => o.Customer!,
                o => o.Employee!,
                o => o.OrderDetails!
            );
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _orderRepo.GetByIdWithIncludeAsync(id,
                o => o.Customer!,
                o => o.Employee!,
                o => o.OrderDetails!
            );
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto, Guid employeeId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var customer = await _customerRepo.GetByIdAsync(dto.CustomerId)
                    ?? throw new KeyNotFoundException("Customer not found");

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    EmployeeId = employeeId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow
                };

                await _orderRepo.AddAsync(order);
                await _orderRepo.SaveChangesAsync();

                decimal total = 0;
                foreach (var item in dto.OrderDetails)
                {
                    var product = await _productRepo.GetByIdWithIncludeAsync(item.ProductId, p => p.Inventory!)
                        ?? throw new KeyNotFoundException($"Product {item.ProductId} not found");

                    if (product.Inventory!.QuantityOnHand < item.Quantity)
                        throw new InvalidOperationException($"Not enough stock for {product.Name}");

                    product.Inventory.QuantityOnHand -= item.Quantity;
                    _inventoryRepo.Update(product.Inventory);

                    var detail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _detailRepo.AddAsync(detail);

                    total += product.Price * item.Quantity;
                }

                order.TotalAmount = total;
                _orderRepo.Update(order);

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
                await _paymentRepo.AddAsync(payment);

                await _orderRepo.SaveChangesAsync();
                await transaction.CommitAsync();

                var created = await _orderRepo.GetByIdWithIncludeAsync(order.Id,
                    o => o.Customer!,
                    o => o.OrderDetails!
                );

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
            var order = await _orderRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Order not found");

            if (!Enum.TryParse<OrderStatus>(newStatus, true, out var status))
                throw new ArgumentException("Invalid status value");

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }
    }
}