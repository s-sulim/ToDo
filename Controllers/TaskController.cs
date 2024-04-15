using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private List<ToDo.Models.Task> GetAllTask()
        {
            return _context.Tasks.Include(t => t.Substeps).ToList();
        }
        private ToDo.Models.Task GetTaskById(int id)
        {
            return GetAllTask().FirstOrDefault(t => t.Id == id);
        }
        [Authorize]
        public IActionResult ViewTask(int id)
        {
            if (id != 0)
            {
                var taskFromDb = GetTaskById(id);
                if (taskFromDb != null)
                {
                    //if ((taskFromDb.CreatedUserName != User.Identity.Name) && !User.IsInRole("Admin"))
                    //{
                    //    return Unauthorized();
                    //}
                    return View(taskFromDb);
                }
                else
                {
                    return NotFound();
                }
            }
            return View(new Models.Task() { DueDate = DateTime.Today,Substeps = new List<Models.Task.Substep>() { new Models.Task.Substep() {IsDone = false, Text = "" } } });
        }
        [Authorize]
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
                foreach (var item in task.Substeps.Where(s => s.MarkedForDeletion))
                {
                    if (_context.Substeps.Any(s=>s.Id == item.Id))
                    {
                        _context.Substeps.Remove(item);
                    }
                }
                _context.Tasks.Update(task);
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [Authorize]
        public IActionResult DeleteTask(int id)
        {
            if (id != 0)
            {
                var taskFromDb = GetTaskById(id);
                if (taskFromDb == null)
                {
                    return NotFound();
                }
                else
                {
                    _context.Tasks.Remove(taskFromDb);
                    foreach (var item in taskFromDb.Substeps)
                    {
                        _context.Substeps.Remove(item);
                    }
                    _context.SaveChanges();
                }
               
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View(GetAllTask());
        }
    }
}
