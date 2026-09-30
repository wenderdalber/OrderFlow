using OrderFlow.Application.Abstractions;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.PlaceOrder;

public sealed class PlaceOrderHandler(IOrderRepository orders, IUnitOfWork unitOfWork)
    : OrderTransitionHandler(orders, unitOfWork)
{
    protected override void Apply(Order order) => order.Place();
}