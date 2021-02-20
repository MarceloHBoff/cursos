using MyTasks.Models;

namespace MyTasks.Repositories.Contracts
{
    public interface ITokenRepository
    {
        void Create(Token token);
        Token Get(string refreshToken);
        void Update(Token token);
    }
}