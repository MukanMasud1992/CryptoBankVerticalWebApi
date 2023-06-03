﻿using CryptoBankVerticalWebApi.Features.Users.Domain;
using CryptoBankVerticalWebApi.Features.Users.Options;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace CryptoBankVerticalWebApi.Features.Users.Services
{
    public class PasswordHeshingService
    {
        private readonly PasswordHashingOptions _options;

        public PasswordHeshingService(IOptions<PasswordHashingOptions> options)
        {
            _options=options.Value;
        }

        public string GenerateSalt()
        {
            byte[] salt = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            
            return Convert.ToBase64String(salt);
        }

        public string GetPasswordHash(string password, byte[] passwordSalt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                DegreeOfParallelism = _options.Parallelism,
                MemorySize = _options.MemorySize,
                Iterations = _options.Iterations,
                Salt = passwordSalt
            };
            {
                byte[] passwordHash = argon2.GetBytes(32);
                return (Convert.ToBase64String(passwordHash));
            }
        }

        public bool CheckPasswordHasherOptions(User user)
        {
            if (user.MemorySize == _options.MemorySize
                && user.Parallelism ==_options.Parallelism
                && user.Iterations == _options.Iterations)
            {
                return true;
            }
            return false;
        }
      
    }
}
