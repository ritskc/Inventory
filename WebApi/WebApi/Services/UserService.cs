using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.IServices;
using DAL.Models;
using DAL.IRepository;
using WebApi.Settings;

namespace WebApi.Services
{
    public class UserService:IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        //private List<User> _users = new List<User>
        //{
        //    new User { Id = 1, FirstName = "Test", LastName = "User", UserName = "admin", Password = "admin" },
        //    new User { Id = 2, FirstName = "Test", LastName = "User", UserName = "parag", Password = "parag" }
        //};

        private readonly AppSettings _appSettings;
        private readonly IUserRepository userRepository;

        public UserService(IOptions<AppSettings> appSettings, IUserRepository userRepository)
        {
            _appSettings = appSettings.Value;
            this.userRepository = userRepository;
        }

        public User Authenticate(string username, string password)
        {
            var _users = GetAllUsersAsync().Result;
            var user = _users.Where(x => x.UserName == username.ToLower() && x.Password == password.ToLower()).FirstOrDefault();

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
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
            user.TokenExpires = tokenDescriptor.Expires;
            // remove password before returning
            user.Password = null;

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            // return users without passwords
            //return _users.Select(x => {
            //    x.Password = null;
            //    return x;
            //});
            return GetAllUsersAsync().Result;
        }

        public async Task<User> GetUserAsync(string userName)
        {
            return await this.userRepository.GetUserPropertyAsync(userName);            
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await this.userRepository.GetAllUsersAsync();
        }

        public async Task<User> GeUserbyIdAsync(int userId)
        {
            return await this.userRepository.GeUserbyIdAsync(userId);
        }

        public async Task AddUserAsync(User user)
        {
            await this.userRepository.AddUserAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            await this.userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(long id)
        {
            await this.userRepository.DeleteUserAsync(id);
        }
    }
}
