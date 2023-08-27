using Data.ApplicationDbContext;
using Data.Models;
using Data.Repository;
using Data.UnitOfWork;
using Data.VMs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Security
{
    public class AuthenticationService : IAuthenticationService
    {
        private const string JWT_SECRET = "f9a32479-4549-4cf2-ba47-daa00c3f2afe";
        private IRepository<User> _userRepo;
        private IUnitOfWork<AppDbContext> _unitOfWork;
        public AuthenticationService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userRepo = _unitOfWork.GetRepository<User>();
        }
        public async Task<GetUserDataResponse> IsAuthenticated(string? UserName, string? Password)
        {
            var user = _userRepo.First(u => u.Username == UserName && u.Password == Password);

            if (user == null)
            {
                return null;
            }
            var result = new GetUserDataResponse()
            {
                UserName = UserName,
                IsAuthenticated = true
            };
            byte[] keyByteArray = Encoding.UTF8.GetBytes(JWT_SECRET);
            SymmetricSecurityKey signingkey = new(keyByteArray);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserName", result.UserName),
                    }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(signingkey, SecurityAlgorithms.HmacSha256),
                NotBefore = DateTime.Now,
                Issuer = "Issuer"
            };
            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);
            
            result.Token = token;

            return result;
        }
    }
}
