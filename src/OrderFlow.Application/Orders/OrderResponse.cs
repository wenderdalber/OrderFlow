using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders;

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    string Status,
    DateTime CreatedAt,
    decimal Total,
    IReadOnlyList<OrderItemResponse> Items);

public sealed record OrderItemResponse(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Total);

internal static class OrderMappings
{
    public static OrderResponse ToResponse(this Order order) => new(
        order.Id,
        order.CustomerId,
        order.Status.ToString(),
        order.CreatedAt,
        order.Total.Amount,
        [.. order.Items.Select(i => new OrderItemResponse(
            i.ProductId,
            i.ProductName,
            i.UnitPrice.Amount,
            i.Quantity,
            i.Total.Amount))]);
}