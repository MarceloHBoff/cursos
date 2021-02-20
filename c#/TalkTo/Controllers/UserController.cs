using System;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using TalkTo.Models;
using TalkTo.Repositories.Contracts;
using Microsoft.AspNetCore.Cors;

namespace TalkTo.Controllers
{
    [Route("api/[controller]")]
    // [EnableCors("Free")]
    public class UserController : ControllerBase
    {
        public readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;

        public UserController(IMapper mapper, UserManager<User> userManager, IUserRepository userRepository, ITokenRepository tokenRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult Get(string id, [FromHeader(Name = "Accept")] string mediaType)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null) return NotFound();

            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Links.Add(new LinkDTO("_self", Url.Link("GetUser", new { id = user.Id }), "GET"));
            userDTO.Links.Add(new LinkDTO("_update", Url.Link("UpdateUser", new { id = user.Id }), "PUT"));

            return Ok(userDTO);
        }

        [Authorize]
        [HttpGet]
        public ActionResult All([FromHeader(Name = "Accept")] string mediaType)
        {
            var users = _userManager.Users.ToList();
            if (mediaType == "application/vnd.talkto.hateoas+json")
            {
                var usersDTO = _mapper.Map<List<UserDTO>>(users);

                for (var i = 0; i < usersDTO.Count; i++)
                {
                    usersDTO[i].Links.Add(new LinkDTO("_self", Url.Link("GetUser", new { id = users[i].Id }), "GET"));
                }

                return Ok(usersDTO);
            }
            else
            {
                return Ok(users);
            }
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

        [HttpPost(Name = "CreateUser")]
        public ActionResult Create([FromBody] UserDTO user, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

            try
            {
                var newUser = _mapper.Map<User>(user);
                _userRepository.Create(newUser, user.Password);

                if (mediaType == "application/vnd.talkto.hateoas+json")
                {
                    var userDTO = _mapper.Map<UserDTO>(newUser);
                    userDTO.Links.Add(new LinkDTO("_get", Url.Link("GetUser", new { id = newUser.Id }), "GET"));
                    userDTO.Links.Add(new LinkDTO("_self", Url.Link("CreateUser", null), "POST"));
                    userDTO.Links.Add(new LinkDTO("_update", Url.Link("UpdateUser", new { id = newUser.Id }), "PUT"));

                    return Ok(userDTO);
                }
                else
                {
                    return Ok(newUser);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}", Name = "UpdateUser")]
        public ActionResult Update([FromBody] UserDTO user, string id, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
            var userModel = _userManager.GetUserAsync(HttpContext.User).Result;

            if (userModel?.Id != id) return Forbid();

            try
            {
                // userModel = _mapper.Map<User>(user);
                userModel.Name = user.Name;
                userModel.UserName = user.Email;
                userModel.Email = user.Email;
                userModel.Slogan = user.Slogan;

                _userRepository.Update(userModel, user);

                if (mediaType == "application/vnd.talkto.hateoas+json")
                {
                    var userDTO = _mapper.Map<UserDTO>(userModel);
                    userDTO.Links.Add(new LinkDTO("_self", Url.Link("UpdateUser", new { id = userModel.Id }), "PUT"));
                    userDTO.Links.Add(new LinkDTO("_get", Url.Link("GetUser", new { id = userModel.Id }), "GET"));

                    return Ok(userDTO);
                }
                else
                {
                    return Ok(userModel);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
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
    }
}
