using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrasturcture.ExternalServices
{
    public class TokenService : ITokenService
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public TokenService(IJwtTokenGenerator jwtTokenGenerator)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> CreateAccessTokenAsync(string email, List<string> roles, DateTime expiration)
        {
            // Generate JWT token here
            var user = new User { Email = email }; // Retrieve user details as needed
            var token = _jwtTokenGenerator.GenerateToken(user, roles, expiration);
            return await Task.FromResult(token); // Simulate async operation
        }
    }
}
