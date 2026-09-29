using OrderFlow.Domain.Common;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.ValueObjects;

using Shouldly;

namespace OrderFlow.Domain.Tests.Orders;

public class OrderTests
{
    private static readonly DateTime Now = new(2026, 9, 28, 12, 0, 0, DateTimeKind.Utc);

    private static Order NewOrder() => Order.Create(Guid.NewGuid(), Now);

    private static Order NewOrderWithItem()
    {
        var order = NewOrder();
        order.AddItem(Guid.NewGuid(), "Book", Money.From(50m), 1);
        return order;
    }

    [Fact]
    public void Create_ShouldStartAsDraft()
    {
        var order = NewOrder();

        order.Status.ShouldBe(OrderStatus.Draft);
        order.Items.ShouldBeEmpty();
        order.Total.ShouldBe(Money.Zero);
    }

    [Fact]
    public void Create_WithEmptyCustomer_ShouldThrow()
    {
        Should.Throw<DomainException>(() => Order.Create(Guid.Empty, Now));
    }

    [Fact]
    public void AddItem_ShouldCalculateTotal()
    {
        var order = NewOrder();

        order.AddItem(Guid.NewGuid(), "Book", Money.From(30m), 2);
        order.AddItem(Guid.NewGuid(), "Pen", Money.From(5m), 3);

        order.Total.ShouldBe(Money.From(75m));
    }

    [Fact]
    public void AddItem_SameProductTwice_ShouldMergeQuantity()
    {
        var order = NewOrder();
        var productId = Guid.NewGuid();

        order.AddItem(productId, "Book", Money.From(10m), 1);
        order.AddItem(productId, "Book", Money.From(10m), 2);

        order.Items.ShouldHaveSingleItem().Quantity.ShouldBe(3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_WithInvalidQuantity_ShouldThrow(int quantity)
    {
        var order = NewOrder();

        Should.Throw<DomainException>(() =>
            order.AddItem(Guid.NewGuid(), "Book", Money.From(10m), quantity));
    }

    [Fact]
    public void RemoveItem_NotInOrder_ShouldThrow()
    {
        var order = NewOrderWithItem();

        Should.Throw<DomainException>(() => order.RemoveItem(Guid.NewGuid()));
    }

    [Fact]
    public void Place_WithoutItems_ShouldThrow()
    {
        Should.Throw<DomainException>(() => NewOrder().Place());
    }

    [Fact]
    public void AddItem_AfterPlaced_ShouldThrow()
    {
        var order = NewOrderWithItem();
        order.Place();

        Should.Throw<DomainException>(() =>
            order.AddItem(Guid.NewGuid(), "Pen", Money.From(5m), 1));
    }

    [Fact]
    public void FullFlow_ShouldReachShipped()
    {
        var order = NewOrderWithItem();

        order.Place();
        order.Pay();
        order.Ship();

        order.Status.ShouldBe(OrderStatus.Shipped);
    }

    [Fact]
    public void Pay_WhenDraft_ShouldThrow()
    {
        Should.Throw<DomainException>(() => NewOrderWithItem().Pay());
    }

    [Fact]
    public void Ship_WhenPlaced_ShouldThrow()
    {
        var order = NewOrderWithItem();
        order.Place();

        Should.Throw<DomainException>(() => order.Ship());
    }

    [Fact]
    public void Cancel_WhenPaid_ShouldCancel()
    {
        var order = NewOrderWithItem();
        order.Place();
        order.Pay();

        order.Cancel();

        order.Status.ShouldBe(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_AfterShipped_ShouldThrow()
    {
        var order = NewOrderWithItem();
        order.Place();
        order.Pay();
        order.Ship();

        Should.Throw<DomainException>(() => order.Cancel());
    }
}