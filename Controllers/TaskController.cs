﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Data;

namespace ToDo.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult OpenTask(int id)
        {
            if (id != 0)
            {
                var taskFromDb = _context.Tasks.SingleOrDefault(j => j.Id == id);
                taskFromDb.Substeps = _context.Substeps.Where(j => j.Id == id).ToList();
                if (taskFromDb != null)
                {
                    if ((taskFromDb.CreatedUserName != User.Identity.Name) && !User.IsInRole("Admin"))
                    {
                        return Unauthorized();
                    }
                    return View(taskFromDb);
                }
                else
                {
                    return NotFound();
                }
            }
            return View(new Models.Task() { DueDate = DateTime.Today,Substeps = new List<Models.Task.Substep>() { new Models.Task.Substep() {IsDone = false, Text = "Substep text" } } });
        }
        public IActionResult CreateEditTask(ToDo.Models.Task task)
        {
            if (task.Id == 0)
            {
                task.AssignedUserName = User.Identity.Name;
                task.CreatedUserName = User.Identity.Name;
                task.CreatedDate = DateTime.Now;
                _context.Tasks.Add(task);
            }
            else
            {
                _context.Tasks.Update(task);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}