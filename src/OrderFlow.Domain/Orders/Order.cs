using OrderFlow.Domain.Common;
using OrderFlow.Domain.ValueObjects;

namespace OrderFlow.Domain.Orders;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _items = [];

    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Money Total => _items.Aggregate(Money.Zero, (sum, item) => sum + item.Total);

    private Order() { } // EF Core

    public static Order Create(Guid customerId, DateTime createdAt)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("Customer is required.");

        return new Order
        {
            CustomerId = customerId,
            Status = OrderStatus.Draft,
            CreatedAt = createdAt
        };
    }

    public void AddItem(Guid productId, string productName, Money unitPrice, int quantity)
    {
        EnsureStatus(OrderStatus.Draft, "Items can only be added to draft orders.");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            existing.IncreaseQuantity(quantity);
            return;
        }

        _items.Add(new OrderItem(productId, productName, unitPrice, quantity));
    }

    public void RemoveItem(Guid productId)
    {
        EnsureStatus(OrderStatus.Draft, "Items can only be removed from draft orders.");

        var item = _items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new DomainException("Item not found in order.");

        _items.Remove(item);
    }

    public void Place()
    {
        EnsureStatus(OrderStatus.Draft, "Only draft orders can be placed.");
        if (_items.Count == 0)
            throw new DomainException("Cannot place an order without items.");

        Status = OrderStatus.Placed;
    }

    public void Pay()
    {
        EnsureStatus(OrderStatus.Placed, "Only placed orders can be paid.");
        Status = OrderStatus.Paid;
    }

    public void Ship()
    {
        EnsureStatus(OrderStatus.Paid, "Only paid orders can be shipped.");
        Status = OrderStatus.Shipped;
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Shipped or OrderStatus.Cancelled)
            throw new DomainException($"Cannot cancel an order with status {Status}.");

        Status = OrderStatus.Cancelled;
    }

    private void EnsureStatus(OrderStatus expected, string message)
    {
        if (Status != expected)
            throw new DomainException(message);
    }
}