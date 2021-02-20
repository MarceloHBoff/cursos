using System.Linq;
using MyTasks.Models;
using MyTasks.Database;
using MyTasks.Repositories.Contracts;

namespace MyTasks.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly MyTasksContext _db;

        public TokenRepository(MyTasksContext db)
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