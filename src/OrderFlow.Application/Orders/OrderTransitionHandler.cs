using OrderFlow.Application.Abstractions;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders;

public abstract class OrderTransitionHandler(IOrderRepository orders, IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await orders.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), orderId);

        Apply(order);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    protected abstract void Apply(Order order);
}