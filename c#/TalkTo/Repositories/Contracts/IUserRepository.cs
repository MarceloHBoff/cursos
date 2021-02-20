using TalkTo.Models;

namespace TalkTo.Repositories.Contracts
{
    public interface IUserRepository
    {
        void Create(User user, string password);
        User Login(string email, string password);
        void Update(User user, UserDTO userDTO);
        User GetById(string id);
    }
}