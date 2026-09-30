using Microsoft.Extensions.DependencyInjection;

using OrderFlow.Application.Orders.CancelOrder;
using OrderFlow.Application.Orders.CreateOrder;
using OrderFlow.Application.Orders.GetOrder;
using OrderFlow.Application.Orders.PayOrder;
using OrderFlow.Application.Orders.PlaceOrder;
using OrderFlow.Application.Orders.ShipOrder;

namespace OrderFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<PlaceOrderHandler>();
        services.AddScoped<GetOrderHandler>();
        services.AddScoped<PayOrderHandler>();
        services.AddScoped<ShipOrderHandler>();
        services.AddScoped<CancelOrderHandler>();
        return services;
    }
}