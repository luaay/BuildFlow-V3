using BuildFlow.Projects.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BuildFlow.Projects.Domain.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var money = Money.Create(1000m, "USD");

        // Assert
        money.Amount.Should().Be(1000m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Create_ShouldNormalizeCurrency_ToUppercase()
    {
        // Act
        var money = Money.Create(500m, "usd");

        // Assert
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Create_ShouldRoundAmount_ToTwoDecimals()
    {
        // Act
        var money = Money.Create(10.129m, "USD");

        // Assert — يُدوّر إلى منزلتين
        money.Amount.Should().Be(10.13m);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrow()
    {
        // Act
        var act = () => Money.Create(-1m, "USD");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCurrency_ShouldThrow(string? currency)
    {
        // Act
        var act = () => Money.Create(100m, currency!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Add_SameCurrency_ShouldSumAmounts()
    {
        // Arrange
        var a = Money.Create(100m, "USD");
        var b = Money.Create(50m, "USD");

        // Act
        var sum = a.Add(b);

        // Assert
        sum.Amount.Should().Be(150m);
        sum.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_DifferentCurrencies_ShouldThrow()
    {
        // Arrange
        var usd = Money.Create(100m, "USD");
        var eur = Money.Create(50m, "EUR");

        // Act
        var act = () => usd.Add(eur);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Equality_SameAmountAndCurrency_ShouldBeEqual()
    {
        // Arrange
        var a = Money.Create(100m, "USD");
        var b = Money.Create(100m, "USD");

        // Assert — كائن القيمة يتساوى بالقيمة لا بالمرجع
        a.Should().Be(b);
    }
}