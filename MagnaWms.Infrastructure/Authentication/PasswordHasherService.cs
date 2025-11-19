using MagnaWms.Application.Core.Abstractions.Authentication;
using Microsoft.AspNetCore.Identity;

namespace MagnaWms.Infrastructure.Authentication;

internal sealed class PasswordHasherService : IPasswordHasherService
{
    // Using Identity's PBKDF2 hasher, but I don't care about TUser here.
    private readonly PasswordHasher<object> _inner = new();

    public string HashPassword(string password) => _inner.HashPassword(new object(), password);

    public bool VerifyPassword(string password, string passwordHash)
    {
        PasswordVerificationResult result =
            _inner.VerifyHashedPassword(new object(), passwordHash, password);

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
