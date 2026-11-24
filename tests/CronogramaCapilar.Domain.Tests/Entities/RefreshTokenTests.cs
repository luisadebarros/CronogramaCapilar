using CronogramaCapilar.Domain.Entities;
using CronogramaCapilar.Domain.Exceptions;

namespace CronogramaCapilar.Domain.Tests.Entities;

public sealed class RefreshTokenTests
{
    private static readonly DateTime Now = new(2026, 11, 24, 12, 0, 0, DateTimeKind.Utc);

    private static RefreshToken CreateToken() => RefreshToken.Create(Guid.NewGuid(), "hash", Now.AddDays(30));

    [Fact]
    public void NewToken_IsActiveUntilItExpires()
    {
        var token = CreateToken();

        Assert.True(token.IsActive(Now));
        Assert.True(token.IsActive(Now.AddDays(30).AddSeconds(-1)));
        Assert.False(token.IsActive(Now.AddDays(30)));
    }

    [Fact]
    public void Revoke_DeactivatesTheToken()
    {
        var token = CreateToken();

        token.Revoke(Now);

        Assert.False(token.IsActive(Now));
        Assert.Equal(Now, token.RevokedAtUtc);
        Assert.False(token.WasReplaced);
    }

    [Fact]
    public void ReplaceWith_RevokesAndStoresTheReplacement()
    {
        var oldToken = CreateToken();
        var newToken = CreateToken();

        oldToken.ReplaceWith(newToken, Now);

        Assert.False(oldToken.IsActive(Now));
        Assert.True(oldToken.WasReplaced);
        Assert.Equal(newToken.Id, oldToken.ReplacedByTokenId);
    }

    [Fact]
    public void ReplaceWith_RevokedToken_ThrowsDomainException()
    {
        var token = CreateToken();
        token.Revoke(Now);

        Assert.Throws<DomainException>(() => token.ReplaceWith(CreateToken(), Now));
    }

    [Fact]
    public void Create_WithoutUser_ThrowsDomainException() =>
        Assert.Throws<DomainException>(() => RefreshToken.Create(Guid.Empty, "hash", Now.AddDays(1)));
}
