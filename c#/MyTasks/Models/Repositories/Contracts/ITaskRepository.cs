using System;
using System.Collections.Generic;
using MyTasks.Models;

namespace MyTasks.Repositories.Contracts
{
    public interface ITaskRepository
    {
        List<Task> Sync(List<Task> tasks);
        List<Task> Restore(User user, DateTime? date);
    }
}