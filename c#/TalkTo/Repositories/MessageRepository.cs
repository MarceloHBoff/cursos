using System;
using System.Linq;
using System.Collections.Generic;
using TalkTo.Models;
using TalkTo.Database;
using TalkTo.Repositories.Contracts;

namespace TalkTo.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly TalkToContext _db;

        public MessageRepository(TalkToContext db)
        {
            _db = db;
        }

        public Message Get(int id)
        {
            return _db.Message.Find(id);
        }

        public List<Message> GetMessages(string receiver, string emitter)
        {
            var messages = _db.Message.Where(m => (m.EmitterId == receiver || m.EmitterId == emitter) && (m.ReceiverId == emitter || m.ReceiverId == receiver)).ToList();
            return messages;
        }

        public void Create(Message message)
        {
            message.CreatedAt = DateTime.Now;
            _db.Message.Add(message);
            _db.SaveChanges();
        }

        public void Update(Message message)
        {
            _db.Message.Update(message);
            _db.SaveChanges();
        }
    }
}