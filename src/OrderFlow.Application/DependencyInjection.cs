using Microsoft.Extensions.DependencyInjection;

using OrderFlow.Application.Orders.CreateOrder;
using OrderFlow.Application.Orders.GetOrder;
using OrderFlow.Application.Orders.PlaceOrder;

namespace OrderFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<PlaceOrderHandler>();
        services.AddScoped<GetOrderHandler>();
        return services;
    }
}