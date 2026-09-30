using OrderFlow.Application.Abstractions;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.CancelOrder;

public sealed class CancelOrderHandler(IOrderRepository orders, IUnitOfWork unitOfWork)
    : OrderTransitionHandler(orders, unitOfWork)
{
    protected override void Apply(Order order) => order.Cancel();
}