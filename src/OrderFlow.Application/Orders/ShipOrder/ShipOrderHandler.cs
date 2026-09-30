using OrderFlow.Application.Abstractions;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.ShipOrder;

public sealed class ShipOrderHandler(IOrderRepository orders, IUnitOfWork unitOfWork)
    : OrderTransitionHandler(orders, unitOfWork)
{
    protected override void Apply(Order order) => order.Ship();
}