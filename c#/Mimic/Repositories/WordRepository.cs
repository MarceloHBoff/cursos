using System;
using System.Linq;
using Mimic.Models;
using Mimic.Helpers;
using Mimic.Database;
using Mimic.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Mimic.Repositories
{
    public class WordRepository : IWordRepository
    {
        public readonly MimicContext _db;

        public WordRepository(MimicContext db)
        {
            _db = db;
        }

        public PaginationList<Word> GetWords(WordUrlQuery query)
        {
            var list = new PaginationList<Word>();

            var words = _db.Words.AsQueryable();
            if (query.Date.HasValue)
            {
                words = words.Where(p => p.CreatedAt > query.Date);
            }

            if (query.Page.HasValue && query.PageSize.HasValue)
            {
                var total = words.Count();
                words = words.Skip((int)((query.Page - 1) * query.PageSize)).Take((int)query.PageSize);

                var pagination = new Pagination();
                pagination.PageNumber = query.Page.Value;
                pagination.PageSize = query.PageSize.Value;
                pagination.Total = total;
                pagination.TotalPage = (int)Math.Ceiling((decimal)(total / query.PageSize.Value));

                list.Pagination = pagination;
            }

            list.Results.AddRange(words.ToList());

            return list;
        }

        public Word Get(int id)
        {
            return _db.Words.AsNoTracking().FirstOrDefault(p => p.Id == id);
        }

        public void Insert(Word word)
        {
            _db.Words.Add(word);
            _db.SaveChanges();
        }

        public void Update(Word word)
        {
            _db.Words.Update(word);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var word = Get(id);
            word.Active = false;
            _db.Words.Update(word);
            _db.SaveChanges();
        }
    }
}