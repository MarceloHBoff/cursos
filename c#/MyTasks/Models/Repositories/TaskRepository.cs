using System;
using System.Linq;
using System.Collections.Generic;
using MyTasks.Models;
using MyTasks.Database;
using MyTasks.Repositories.Contracts;

namespace MyTasks.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly MyTasksContext _db;

        public TaskRepository(MyTasksContext db)
        {
            _db = db;
        }

        public List<Task> Restore(User user, DateTime? date)
        {
            var query = _db.Tasks.Where(t => t.UserId == user.Id).AsQueryable();

            if (date != null)
            {
                query.Where(t => t.CreatedAt >= date || t.UpdatedAt >= date);
            }

            return query.ToList();
        }

        public List<Task> Sync(List<Task> tasks)
        {
            var newTasks = tasks.Where(t => t.Id == 0).ToList();
            var excludedOrUpdatedTasks = tasks.Where(t => t.Id != 0).ToList();

            foreach (var task in newTasks)
            {
                _db.Tasks.Add(task);
            }

            foreach (var task in excludedOrUpdatedTasks)
            {
                _db.Tasks.Update(task);
            }

            _db.SaveChanges();

            return newTasks.ToList();
        }
    }
}