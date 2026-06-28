using BuildFlow.Identity.Application.Abstractions;

namespace BuildFlow.Identity.Infrastructure.Authentication;

internal sealed class PasswordHasher : IPasswordHasher
{
    // work factor — كلّما زاد، أبطأ وأأمن (12 توازن جيد)
    private const int WorkFactor = 12;

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}