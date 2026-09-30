using Microsoft.AspNetCore.Mvc;

using OrderFlow.Application.Orders;
using OrderFlow.Application.Orders.CreateOrder;
using OrderFlow.Application.Orders.GetOrder;
using OrderFlow.Application.Orders.PlaceOrder;
using OrderFlow.Application.Orders.CancelOrder;
using OrderFlow.Application.Orders.PayOrder;
using OrderFlow.Application.Orders.ShipOrder;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Produces("application/json")]
public sealed class OrdersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreateOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        CreateOrderCommand command,
        [FromServices] CreateOrderHandler handler,
        CancellationToken cancellationToken)
    {
        var id = await handler.HandleAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new CreateOrderResponse(id));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromServices] GetOrderHandler handler,
        CancellationToken cancellationToken)
    {
        var order = await handler.HandleAsync(id, cancellationToken);
        return Ok(order);
    }

    [HttpPost("{id:guid}/place")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Place(
        Guid id,
        [FromServices] PlaceOrderHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/pay")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Pay(
    Guid id,
    [FromServices] PayOrderHandler handler,
    CancellationToken cancellationToken)
    {
        await handler.HandleAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/ship")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Ship(
        Guid id,
        [FromServices] ShipOrderHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromServices] CancelOrderHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(id, cancellationToken);
        return NoContent();
    }
}

public sealed record CreateOrderResponse(Guid Id);