using BuildFlow.Identity.Domain.Users;
using FluentAssertions;
using Xunit;

namespace BuildFlow.Identity.Domain.UnitTests.Users;

public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldSucceed()
    {
        // Arrange
        var input = "user@example.com";

        // Act
        var result = Email.Create(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void Create_ShouldNormalizeEmail_ToLowercaseAndTrimmed()
    {
        // Arrange
        var input = "  User@Example.COM  ";

        // Act
        var result = Email.Create(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("user@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyEmail_ShouldFail(string? input)
    {
        // Act
        var result = Email.Create(input);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@domain")]
    [InlineData("@nodomain.com")]
    [InlineData("spaces in@email.com")]
    public void Create_WithInvalidFormat_ShouldFail(string input)
    {
        // Act
        var result = Email.Create(input);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void TwoEmails_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var email1 = Email.Create("user@example.com").Value;
        var email2 = Email.Create("USER@example.com").Value;

        // Act & Assert — structural equality من ValueObject
        email1.Should().Be(email2);
    }
}