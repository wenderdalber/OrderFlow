using NSubstitute;

using OrderFlow.Application.Abstractions;
using OrderFlow.Application.Common;
using OrderFlow.Application.Orders.GetOrder;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.ValueObjects;

using Shouldly;

namespace OrderFlow.Application.Tests.Orders;

public class GetOrderHandlerTests
{
    private readonly IOrderRepository _orders = Substitute.For<IOrderRepository>();
    private readonly GetOrderHandler _handler;

    public GetOrderHandlerTests()
    {
        _handler = new GetOrderHandler(_orders);
    }

    [Fact]
    public async Task HandleAsync_WhenOrderNotFound_ShouldThrowNotFound()
    {
        _orders.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        await Should.ThrowAsync<NotFoundException>(() => _handler.HandleAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task HandleAsync_ShouldMapOrderToResponse()
    {
        var productId = Guid.NewGuid();
        var order = Order.Create(Guid.NewGuid(), DateTime.UtcNow);
        order.AddItem(productId, "Book", Money.From(30m), 2);

        _orders.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var response = await _handler.HandleAsync(order.Id);

        response.Id.ShouldBe(order.Id);
        response.CustomerId.ShouldBe(order.CustomerId);
        response.Status.ShouldBe(nameof(OrderStatus.Draft));
        response.Total.ShouldBe(60m);

        var item = response.Items.ShouldHaveSingleItem();
        item.ProductId.ShouldBe(productId);
        item.UnitPrice.ShouldBe(30m);
        item.Quantity.ShouldBe(2);
        item.Total.ShouldBe(60m);
    }
}