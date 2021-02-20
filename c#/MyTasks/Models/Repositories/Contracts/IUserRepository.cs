using MyTasks.Models;

namespace MyTasks.Repositories.Contracts
{
    public interface IUserRepository
    {
        void Register(User user, string password);
        User Login(string email, string password);
        User GetById(string id);
    }
}