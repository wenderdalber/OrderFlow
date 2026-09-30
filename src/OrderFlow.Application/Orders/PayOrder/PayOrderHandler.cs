using OrderFlow.Application.Abstractions;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.PayOrder;

public sealed class PayOrderHandler(IOrderRepository orders, IUnitOfWork unitOfWork)
    : OrderTransitionHandler(orders, unitOfWork)
{
    protected override void Apply(Order order) => order.Pay();
}