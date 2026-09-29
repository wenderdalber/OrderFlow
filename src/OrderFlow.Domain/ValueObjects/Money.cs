using OrderFlow.Domain.Common;

namespace OrderFlow.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }

    private Money(decimal amount) => Amount = amount;

    public static Money Zero => new(0m);

    public static Money From(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative.");

        return new Money(decimal.Round(amount, 2, MidpointRounding.AwayFromZero));
    }

    public static Money operator +(Money left, Money right) => new(left.Amount + right.Amount);
    public static Money operator *(Money money, int quantity) => new(money.Amount * quantity);
}