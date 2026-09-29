using OrderFlow.Domain.Common;
using OrderFlow.Domain.ValueObjects;

using Shouldly;

namespace OrderFlow.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void From_WithNegativeAmount_ShouldThrow()
    {
        Should.Throw<DomainException>(() => Money.From(-1m));
    }

    [Fact]
    public void From_ShouldRoundToTwoDecimals()
    {
        Money.From(10.005m).Amount.ShouldBe(10.01m);
    }

    [Fact]
    public void Add_ShouldSumAmounts()
    {
        (Money.From(10m) + Money.From(5.50m)).ShouldBe(Money.From(15.50m));
    }

    [Fact]
    public void Multiply_ShouldMultiplyByQuantity()
    {
        (Money.From(2.50m) * 3).ShouldBe(Money.From(7.50m));
    }
}