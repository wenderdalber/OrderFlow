using Microsoft.Extensions.Time.Testing;

using NSubstitute;

using OrderFlow.Application.Abstractions;
using OrderFlow.Application.Orders.CreateOrder;
using OrderFlow.Domain.Common;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.ValueObjects;

using Shouldly;

namespace OrderFlow.Application.Tests.Orders;

public class CreateOrderHandlerTests
{
    private readonly IOrderRepository _orders = Substitute.For<IOrderRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(2026, 9, 28, 12, 0, 0, TimeSpan.Zero));
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _handler = new CreateOrderHandler(_orders, _unitOfWork, _time);
    }

    [Fact]
    public async Task HandleAsync_ShouldPersistOrderWithItems()
    {
        var command = new CreateOrderCommand(Guid.NewGuid(),
        [
            new CreateOrderItem(Guid.NewGuid(), "Book", 30m, 2),
            new CreateOrderItem(Guid.NewGuid(), "Pen", 5m, 1)
        ]);

        var orderId = await _handler.HandleAsync(command);

        await _orders.Received(1).AddAsync(
            Arg.Is<Order>(o =>
                o.Id == orderId &&
                o.CustomerId == command.CustomerId &&
                o.Items.Count == 2 &&
                o.Total == Money.From(65m) &&
                o.CreatedAt == _time.GetUtcNow().UtcDateTime),
            Arg.Any<CancellationToken>());

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithInvalidItem_ShouldThrowAndNotSave()
    {
        var command = new CreateOrderCommand(Guid.NewGuid(),
        [
            new CreateOrderItem(Guid.NewGuid(), "Book", 30m, 0)
        ]);

        await Should.ThrowAsync<DomainException>(() => _handler.HandleAsync(command));

        await _orders.DidNotReceive().AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}