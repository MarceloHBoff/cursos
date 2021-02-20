using System.Collections.Generic;
using TalkTo.Models;

namespace TalkTo.Repositories.Contracts
{
    public interface IMessageRepository
    {
        List<Message> GetMessages(string receiver, string emitter);
        Message Get(int id);
        void Create(Message message);
        void Update(Message message);
    }
}