using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTasks.Models;
using MyTasks.Repositories.Contracts;

namespace MyTasks.Controllers
{
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly UserManager<User> _userManager;

        public TaskController(ITaskRepository taskRepository, UserManager<User> userManager)
        {
            _taskRepository = taskRepository;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public ActionResult Sync([FromBody] List<Task> tasks)
        {
            return Ok(_taskRepository.Sync(tasks));
        }

        [Authorize]
        [HttpGet]
        public ActionResult Restore(DateTime? data)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            return Ok(_taskRepository.Restore(user, data));
        }
    }
}
