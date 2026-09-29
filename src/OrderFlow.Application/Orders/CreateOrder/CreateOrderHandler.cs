using OrderFlow.Application.Abstractions;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Application.Orders.CreateOrder;

public sealed class CreateOrderHandler(
    IOrderRepository orders,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
{
    public async Task<Guid> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = Order.Create(command.CustomerId, timeProvider.GetUtcNow().UtcDateTime);

        foreach (var item in command.Items)
            order.AddItem(item.ProductId, item.ProductName, Money.From(item.UnitPrice), item.Quantity);

        await orders.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}