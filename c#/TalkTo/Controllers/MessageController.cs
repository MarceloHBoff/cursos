using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using TalkTo.Models;
using TalkTo.Repositories.Contracts;
using Microsoft.AspNetCore.Cors;

namespace TalkTo.Controllers
{
    [Route("api/[controller]")]
    // [EnableCors]
    public class MessageController : ControllerBase
    {
        public readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;

        public MessageController(IMapper mapper, IMessageRepository messageRepository)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        [Authorize]
        [HttpGet("{receiverId}/{emitterId}")]
        public ActionResult Get(string receiverId, string emitterId)
        {
            if (receiverId == emitterId) return UnprocessableEntity();
            return Ok(_messageRepository.GetMessages(receiverId, emitterId));
        }

        [Authorize]
        [HttpPost(Name = "CreateMessage")]
        public ActionResult Create([FromBody] Message message, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!ModelState.IsValid) return UnprocessableEntity();

            _messageRepository.Create(message);

            if (mediaType == "application/vnd.talkto.hateoas+json")
            {
                var messageDTO = _mapper.Map<MessageDTO>(message);
                messageDTO.Links.Add(new LinkDTO("_self", Url.Link("CreateMessage", null), "POST"));
                messageDTO.Links.Add(new LinkDTO("_update", Url.Link("PartialUpdateMessage", new { id = message.Id }), "PATCH"));

                return Ok(messageDTO);
            }
            else
            {
                return Ok(message);
            }
        }

        [Authorize]
        [HttpPatch("{id}", Name = "PartialUpdateMessage")]
        public ActionResult PartialUpdate(int id, [FromBody] JsonPatchDocument<Message> jsonPatch, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (jsonPatch == null) return BadRequest();

            var message = _messageRepository.Get(id);

            jsonPatch.ApplyTo(message);
            message.UpdatedAt = DateTime.Now;

            _messageRepository.Update(message);

            if (mediaType == "application/vnd.talkto.hateoas+json")
            {
                var messageDTO = _mapper.Map<MessageDTO>(message);
                messageDTO.Links.Add(new LinkDTO("_self", Url.Link("PartialUpdateMessage", new { id = message.Id }), "PATCH"));

                return Ok(messageDTO);
            }
            else
            {
                return Ok(message);
            }
        }
    }
}
