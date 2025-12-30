namespace FTT_VENDER_API.Common
{
    using Microsoft.AspNetCore.Identity;

    public class PasswordService
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string plainPassword)
        {
            return _hasher.HashPassword(null, plainPassword);
        }

        public bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            var result = _hasher.VerifyHashedPassword(null, hashedPassword, inputPassword);
            return result == PasswordVerificationResult.Success;
        }
    }

}
