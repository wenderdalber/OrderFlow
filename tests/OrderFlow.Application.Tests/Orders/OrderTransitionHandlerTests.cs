using NSubstitute;

using OrderFlow.Application.Abstractions;
using OrderFlow.Application.Common;
using OrderFlow.Application.Orders;
using OrderFlow.Application.Orders.CancelOrder;
using OrderFlow.Application.Orders.PayOrder;
using OrderFlow.Application.Orders.PlaceOrder;
using OrderFlow.Application.Orders.ShipOrder;
using OrderFlow.Domain.Common;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.ValueObjects;

using Shouldly;

namespace OrderFlow.Application.Tests.Orders;

public class OrderTransitionHandlerTests
{
    private readonly IOrderRepository _orders = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public static TheoryData<string> AllTransitions => ["place", "pay", "ship", "cancel"];

    public static TheoryData<string, OrderStatus, OrderStatus> ValidTransitions => new()
    {
        { "place",  OrderStatus.Draft,  OrderStatus.Placed },
        { "pay",    OrderStatus.Placed, OrderStatus.Paid },
        { "ship",   OrderStatus.Paid,   OrderStatus.Shipped },
        { "cancel", OrderStatus.Placed, OrderStatus.Cancelled }
    };

    [Theory]
    [MemberData(nameof(AllTransitions))]
    public async Task HandleAsync_WhenOrderNotFound_ShouldThrowNotFound(string transition)
    {
        _orders.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        await Should.ThrowAsync<NotFoundException>(
            () => CreateHandler(transition).HandleAsync(Guid.NewGuid()));

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [MemberData(nameof(ValidTransitions))]
    public async Task HandleAsync_WithValidTransition_ShouldUpdateStatusAndSave(
        string transition, OrderStatus from, OrderStatus expected)
    {
        var order = CreateOrderIn(from);
        _orders.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        await CreateHandler(transition).HandleAsync(order.Id);

        order.Status.ShouldBe(expected);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithInvalidTransition_ShouldThrowAndNotSave()
    {
        var order = CreateOrderIn(OrderStatus.Draft);
        _orders.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        await Should.ThrowAsync<DomainException>(
            () => CreateHandler("ship").HandleAsync(order.Id));

        order.Status.ShouldBe(OrderStatus.Draft);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private OrderTransitionHandler CreateHandler(string transition) => transition switch
    {
        "place" => new PlaceOrderHandler(_orders, _unitOfWork),
        "pay" => new PayOrderHandler(_orders, _unitOfWork),
        "ship" => new ShipOrderHandler(_orders, _unitOfWork),
        "cancel" => new CancelOrderHandler(_orders, _unitOfWork),
        _ => throw new ArgumentOutOfRangeException(nameof(transition), transition, null)
    };

    private static Order CreateOrderIn(OrderStatus status)
    {
        var order = Order.Create(Guid.NewGuid(), DateTime.UtcNow);
        order.AddItem(Guid.NewGuid(), "Book", Money.From(50m), 1);

        if (status >= OrderStatus.Placed) order.Place();
        if (status >= OrderStatus.Paid) order.Pay();
        if (status >= OrderStatus.Shipped) order.Ship();

        return order;
    }
}