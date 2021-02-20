using System;
using MyTasks.Models;
using MyTasks.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace MyTasks.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public User Login(string email, string password)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            if (_userManager.CheckPasswordAsync(user, password).Result)
            {
                return user;
            }

            throw new Exception("User not found!");
        }

        public User GetById(string id)
        {
            return _userManager.FindByIdAsync(id).Result;
        }

        public void Register(User user, string password)
        {
            var result = _userManager.CreateAsync(user, password).Result;
            if (!result.Succeeded)
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (var error in result.Errors) stringBuilder.Append(error.Description);

                throw new Exception($"User not register! {stringBuilder.ToString()}");
            }
        }
    }
}