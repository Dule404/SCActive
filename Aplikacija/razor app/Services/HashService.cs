using System;
using System.Text;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace backend.Services
{

    public class HashService : IHashService 
    {
        private IConfiguration _config;

        public HashService(IConfiguration config)
        {
            _config = config;
        }

        public string HashString(string text)
        {
            byte[] salt = Encoding.ASCII.GetBytes(_config["Passwords:Salt"]);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: text,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 128));

            return hashed;
        }

        public bool VerifyString(string? text, string? hashedText)
        {
            if (text == null || hashedText == null)
                return false;

            return hashedText == HashString(text);
        }
    }
}