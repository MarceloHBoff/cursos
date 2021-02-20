using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Mimic.Helpers;
using Mimic.Models;
using Mimic.Models.DTO;
using Mimic.Repositories.Contracts;
using Newtonsoft.Json;

namespace Mimic.Controllers
{
    [Route("api/[controller]")]
    public class WordsController : ControllerBase
    {
        public readonly IWordRepository _wordReposirory;
        public readonly IMapper _mapper;

        public WordsController(IWordRepository wordReposirory, IMapper mapper)
        {
            _wordReposirory = wordReposirory;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all words
        /// </summary>
        /// <param name="query">Date filter and pagination</param>
        /// <returns>List of words</returns>
        [HttpGet("", Name = "GetWords")]
        public ActionResult GetWords(WordUrlQuery query)
        {
            var wordsList = _wordReposirory.GetWords(query);

            if (wordsList.Results.Count == 0)
            {
                return NotFound();
            }

            var wordDTOList = _mapper.Map<PaginationList<Word>, PaginationList<WordDTO>>(wordsList);

            foreach (var word in wordDTOList.Results)
            {
                word.Links.Add(new LinkDTO("self", Url.Link("Get", new { id = word.Id }), "GET"));
            }

            wordDTOList.Links.Add(new LinkDTO("self", Url.Link("GetWords", query), "GET"));

            if (wordsList.Pagination != null)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(wordsList.Pagination));

                if (query.Page + 1 <= wordsList.Pagination.TotalPage)
                {
                    var queryNext = new WordUrlQuery() { PageSize = query.PageSize, Page = query.Page + 1, Date = query.Date };
                    wordDTOList.Links.Add(new LinkDTO("next", Url.Link("GetWords", queryNext), "GET"));
                }

                if (query.Page - 1 > 0)
                {
                    var queryPrev = new WordUrlQuery() { PageSize = query.PageSize, Page = query.Page - 1, Date = query.Date };
                    wordDTOList.Links.Add(new LinkDTO("prev", Url.Link("GetWords", queryPrev), "GET"));
                }
            }

            return Ok(wordDTOList);
        }

        /// <summary>
        /// Get only one word
        /// </summary>
        /// <param name="id">Word Id</param>
        /// <returns>Word</returns>
        [HttpGet("{id}", Name = "Get")]
        public ActionResult Get(int id)
        {
            var word = _wordReposirory.Get(id);
            if (word == null) return NotFound();

            var wordDTO = _mapper.Map<Word, WordDTO>(word);
            wordDTO.Links.Add(new LinkDTO("self", Url.Link("Get", new { id = id }), "GET"));
            wordDTO.Links.Add(new LinkDTO("update", Url.Link("Update", new { id = id }), "PUT"));
            wordDTO.Links.Add(new LinkDTO("delete", Url.Link("Delete", new { id = id }), "DELETE"));

            return Ok(wordDTO);
        }

        /// <summary>
        /// Create a new Word
        /// </summary>
        /// <param name="word">Word</param>
        /// <returns>Word</returns>
        [HttpPost("")]
        public ActionResult Insert([FromBody] Word word)
        {
            if (word == null) return BadRequest();
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

            _wordReposirory.Insert(word);

            var wordDTO = _mapper.Map<Word, WordDTO>(word);
            wordDTO.Links.Add(new LinkDTO("self", Url.Link("Get", new { id = word.Id }), "GET"));

            return Created($"/api/words/{word.Id}", wordDTO);
        }

        /// <summary>
        /// Update an existing word
        /// </summary>
        /// <param name="id">Word Id</param>
        /// <param name="word">Word updated</param>
        /// <returns>Word</returns>
        [HttpPut("{id}", Name = "Update")]
        public ActionResult Update(int id, [FromBody] Word word)
        {
            var findWord = _wordReposirory.Get(id);
            if (findWord == null) return NotFound();

            if (word == null) return BadRequest();
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

            word.Id = id;
            word.Active = findWord.Active;
            word.CreatedAt = findWord.CreatedAt;
            _wordReposirory.Update(word);

            var wordDTO = _mapper.Map<Word, WordDTO>(word);
            wordDTO.Links.Add(new LinkDTO("self", Url.Link("Get", new { id = word.Id }), "GET"));

            return Ok(wordDTO);
        }

        /// <summary>
        /// Disable an existing word
        /// </summary>
        /// <param name="id">Word Id</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "Delete")]
        public ActionResult Delete(int id)
        {
            var word = _wordReposirory.Get(id);
            if (word == null) return NotFound();

            _wordReposirory.Delete(id);

            return NoContent();
        }
    }
}