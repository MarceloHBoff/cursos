using System.Linq;
using TalkTo.Models;
using TalkTo.Database;
using TalkTo.Repositories.Contracts;

namespace TalkTo.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly TalkToContext _db;

        public TokenRepository(TalkToContext db)
        {
            _db = db;
        }

        public Token Get(string refreshToken)
        {
            return _db.Token.FirstOrDefault(t => t.RefreshToken == refreshToken && !t.Used);
        }

        public void Create(Token token)
        {
            _db.Token.Add(token);
            _db.SaveChanges();
        }

        public void Update(Token token)
        {
            _db.Token.Update(token);
            _db.SaveChanges();
        }
    }
}