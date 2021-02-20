using Mimic.Helpers;
using Mimic.Models;

namespace Mimic.Repositories.Contracts
{
    public interface IWordRepository
    {
        PaginationList<Word> GetWords(WordUrlQuery query);
        Word Get(int id);
        void Insert(Word word);
        void Update(Word word);
        void Delete(int id);
    }
}