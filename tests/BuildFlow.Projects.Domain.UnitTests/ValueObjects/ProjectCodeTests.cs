using BuildFlow.Projects.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BuildFlow.Projects.Domain.UnitTests.ValueObjects;

public class ProjectCodeTests
{
    [Fact]
    public void Create_WithValidCode_ShouldSucceed()
    {
        // Act
        var code = ProjectCode.Create("PRJ-001");

        // Assert
        code.Value.Should().Be("PRJ-001");
    }

    [Fact]
    public void Create_ShouldNormalize_ToUppercase()
    {
        // Act
        var code = ProjectCode.Create("prj-001");

        // Assert
        code.Value.Should().Be("PRJ-001");
    }

    [Fact]
    public void Create_ShouldTrim_Whitespace()
    {
        // Act
        var code = ProjectCode.Create("  PRJ-001  ");

        // Assert
        code.Value.Should().Be("PRJ-001");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCode_ShouldThrow(string? code)
    {
        // Act
        var act = () => ProjectCode.Create(code!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("AB")]                    // قصير جداً، أقلّ من ثلاثة
    [InlineData("THIS-CODE-IS-WAY-TOO-LONG")] // طويل جداً، أكثر من عشرين
    [InlineData("PRJ_001")]               // شرطة سفلية غير مسموحة
    [InlineData("PRJ 001")]               // مسافة غير مسموحة
    [InlineData("PRJ@001")]               // رمز غير مسموح
    public void Create_WithInvalidFormat_ShouldThrow(string code)
    {
        // Act
        var act = () => ProjectCode.Create(code);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        // Arrange
        var a = ProjectCode.Create("PRJ-001");
        var b = ProjectCode.Create("prj-001"); // يُطبّع للكبير

        // Assert — يتساويان بعد التطبيع
        a.Should().Be(b);
    }
}