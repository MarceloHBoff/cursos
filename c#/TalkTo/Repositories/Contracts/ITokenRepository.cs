using TalkTo.Models;

namespace TalkTo.Repositories.Contracts
{
    public interface ITokenRepository
    {
        void Create(Token token);
        Token Get(string refreshToken);
        void Update(Token token);
    }
}