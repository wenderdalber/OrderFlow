namespace OrderFlow.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(Guid CustomerId, IReadOnlyList<CreateOrderItem> Items);

public sealed record CreateOrderItem(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);