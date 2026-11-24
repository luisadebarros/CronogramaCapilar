using CronogramaCapilar.Domain.Exceptions;
using CronogramaCapilar.Domain.ValueObjects;

namespace CronogramaCapilar.Domain.Tests.ValueObjects;

public sealed class EmailTests
{
    [Fact]
    public void Create_TrimsAndLowercases() =>
        Assert.Equal("maria@email.com", Email.Create("  Maria@Email.COM ").Value);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("no-at-sign")]
    [InlineData("maria@nodot")]
    [InlineData("ma ria@email.com")]
    public void Create_WithInvalidValue_ThrowsDomainException(string? value) =>
        Assert.Throws<DomainException>(() => Email.Create(value));

    [Fact]
    public void TwoEmailsWithSameValue_AreEqual() =>
        Assert.Equal(Email.Create("maria@email.com"), Email.Create("MARIA@email.com"));
}
