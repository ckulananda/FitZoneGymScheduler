using Microsoft.AspNetCore.Identity;

namespace FitZoneGymScheduler.Services
{
    public static class PasswordService
    {
        private static PasswordHasher<object> hasher = new();

        // HASH PASSWORD (STORE THIS IN DB)
        public static string HashPassword(string password)
        {
            return hasher.HashPassword(null, password);
        }

        // VERIFY PASSWORD (LOGIN CHECK)
        public static bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            var result = hasher.VerifyHashedPassword(
                null,
                hashedPassword,
                inputPassword);

            return result == PasswordVerificationResult.Success;
        }
    }
}