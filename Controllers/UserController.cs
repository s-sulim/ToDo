using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using ToDo.Data;
using ToDo.Models;

namespace ToDo.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            IdentityUser user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

  
        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public List<IdentityUser> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public JsonResult GetAllUsersJson()
        {
            var users = _context.Users.Select(u => new { u.Id, u.UserName }).ToList();
            return Json(users);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var userRoleViewModel = new UserRolesViewModel
                {
                    UserName = user.UserName,
                    UserId = user.Id,
                    Roles = _roleManager.Roles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList()
                };

                var userRoles = await _userManager.GetRolesAsync(user);
                userRoleViewModel.SelectedRole = userRoles.FirstOrDefault();

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoles(List<UserRolesViewModel> model)
        {
            foreach (var userRole in model)
            {
                var user = await _userManager.FindByNameAsync(userRole.UserName);
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);

                if (!string.IsNullOrEmpty(userRole.SelectedRole))
                {
                    await _userManager.AddToRoleAsync(user, userRole.SelectedRole);
                }
            }

            return RedirectToAction("Index");
        }
    }
}