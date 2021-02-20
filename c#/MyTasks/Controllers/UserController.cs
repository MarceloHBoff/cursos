using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyTasks.Models;
using MyTasks.Repositories.Contracts;

namespace MyTasks.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;

        public UserController(IUserRepository userRepository, ITokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
        }

        [HttpPost("[action]")]
        public ActionResult Login([FromBody] UserDTO user)
        {
            ModelState.Remove("Name");
            ModelState.Remove("PasswordConfirmation");

            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

            try
            {
                var loggedUser = _userRepository.Login(user.Email, user.Password);
                var token = GetNewToken(loggedUser);

                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost("[action]")]
        public ActionResult Refresh([FromBody] TokenDTO tokenDTO)
        {
            var refreshToken = _tokenRepository.Get(tokenDTO.RefreshToken);

            if (refreshToken == null) return NotFound();

            refreshToken.UpdatedAt = DateTime.Now;
            refreshToken.Used = true;
            _tokenRepository.Update(refreshToken);

            var token = GetNewToken(_userRepository.GetById(refreshToken.UserId));

            return Ok(token);
        }

        [HttpPost("")]
        public ActionResult Register([FromBody] UserDTO user)
        {
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

            try
            {
                var newUser = new User();
                newUser.Name = user.Name;
                newUser.UserName = user.Email;
                newUser.Email = user.Email;
                _userRepository.Register(newUser, user.Password);

                return Ok(newUser);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private TokenDTO BuildToken(User user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("key-key-key-key-key-key-key-key"));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                expires: exp,
                signingCredentials: sign
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = Guid.NewGuid().ToString();
            var expirationRefreshToken = DateTime.UtcNow.AddHours(2);

            var tokenDTO = new TokenDTO
            {
                Token = tokenString,
                Expiration = exp,
                RefreshToken = refreshToken,
                ExpirationRefreshToken = expirationRefreshToken
            };

            return tokenDTO;
        }

        private TokenDTO GetNewToken(User user)
        {
            var token = BuildToken(user);

            var newToken = new Token()
            {
                RefreshToken = token.RefreshToken,
                ExpirationToken = token.Expiration,
                ExpirationRefreshToken = token.ExpirationRefreshToken,
                User = user,
                CreatedAt = DateTime.Now,
                Used = false
            };

            _tokenRepository.Create(newToken);

            return token;
        }
    }
}
