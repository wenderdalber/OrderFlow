using NSubstitute;

using OrderFlow.Application.Abstractions;
using OrderFlow.Application.Common;
using OrderFlow.Application.Orders.PlaceOrder;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.ValueObjects;

using Shouldly;

namespace OrderFlow.Application.Tests.Orders;

public class PlaceOrderHandlerTests
{
    private readonly IOrderRepository _orders = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly PlaceOrderHandler _handler;

    public PlaceOrderHandlerTests()
    {
        _handler = new PlaceOrderHandler(_orders, _unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WhenOrderNotFound_ShouldThrowNotFound()
    {
        _orders.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        await Should.ThrowAsync<NotFoundException>(() => _handler.HandleAsync(Guid.NewGuid()));

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPlaceOrderAndSave()
    {
        var order = Order.Create(Guid.NewGuid(), DateTime.UtcNow);
        order.AddItem(Guid.NewGuid(), "Book", Money.From(50m), 1);

        _orders.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        await _handler.HandleAsync(order.Id);

        order.Status.ShouldBe(OrderStatus.Placed);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}