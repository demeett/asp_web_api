using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using jwtCoreDemo.Entities;
using jwtCoreDemo.Helpers;

namespace WebApi.Services
{
    public interface IUserService
    {
        User Authenticate(string kullaniciAdi, string sifre);
        IEnumerable<User> GetAll();
        IEnumerable<User> Insert(User user);
        bool IsUserExist(User user);
    }

    public class UserService : IUserService
    {
        private List<User> _users = new List<User>
        {
            new User { Id = 1, Ad = "Demet", Soyad = "Demir", KullaniciAdi = "demet12", Sifre = "1234" },
            new User { Id = 2, Ad = "Büşra", Soyad = "Arslan", KullaniciAdi = "busra06", Sifre = "4321" }
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public User Authenticate(string kullaniciAdi, string sifre)
        {
            var user = _users.SingleOrDefault(x => x.KullaniciAdi == kullaniciAdi && x.Sifre == sifre);

            
            if (user == null)
                return null;

           .
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

           
            user.Sifre = null;

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            
            return _users.Select(x =>
            {
                x.Sifre = null;
                return x;
            });
        }

        public bool IsUserExist(User user)
        {
            bool isExist;

            var userName = user.KullaniciAdi.ToLower();
            isExist = _users.Any(n => n.KullaniciAdi == user.KullaniciAdi.ToLower());

            return isExist;
        }

        public IEnumerable<User> Insert(User user)
        {
            _users.Add(user);
            return _users;
        }

        
    }
}