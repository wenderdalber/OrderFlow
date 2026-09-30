using OrderFlow.Application.Abstractions;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.GetOrder;

public sealed class GetOrderHandler(IOrderRepository orders)
{
    public async Task<OrderResponse> HandleAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await orders.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), orderId);

        return order.ToResponse();
    }
}