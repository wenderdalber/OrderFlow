using OrderFlow.Application.Abstractions;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.PlaceOrder;

public sealed class PlaceOrderHandler(IOrderRepository orders, IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await orders.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), orderId);

        order.Place();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}