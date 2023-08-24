using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class UserController : Controller
    {
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;

		public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
			_context = context;
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Update(string? Id)
        {
            if (Id == null || Id == "")
            {
                return View();
            }

			string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == Id).RoleId;

			ApplicationUserVM applicationUserVM = new()
			{
				ApplicationUser = _context.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == Id),
				Companies = _context.Companies.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }),
				Roles = _context.Roles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }),
			};

			applicationUserVM.ApplicationUser.Role = _context.Roles.FirstOrDefault(r => r.Id == roleId).Name;

			return View(applicationUserVM);
		}
        [HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Update(ApplicationUserVM applicationUserVM)
        {
			string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == applicationUserVM.ApplicationUser.Id).RoleId;
			string oldRole = _context.Roles.FirstOrDefault(r => r.Id == roleId).Name;

			if (applicationUserVM.ApplicationUser.Role != oldRole)
			{
				ApplicationUser applicationUser = _context.ApplicationUsers.FirstOrDefault(u => u.Id == applicationUserVM.ApplicationUser.Id);
				if (applicationUserVM.ApplicationUser.Role == StaticDetails.Role_Company)
				{
					applicationUser.CompanyId = applicationUserVM.ApplicationUser.CompanyId;
				}
				if (oldRole == StaticDetails.Role_Company)
				{
					applicationUser.CompanyId = null;
				}

				_context.ApplicationUsers.Update(applicationUser);
				_context.SaveChanges();
				_userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
				_userManager.AddToRoleAsync(applicationUser, applicationUserVM.ApplicationUser.Role).GetAwaiter().GetResult();
			}

			TempData["success"] = "User updated successfully!";
			return RedirectToAction("Index");
		}

		#region API CALL
		[HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<ApplicationUser> UserList = _context.ApplicationUsers.Include(u => u.Company).ToList();
            
			var userRoles = _context.UserRoles.ToList();
			var roles = _context.Roles.ToList();

			foreach (ApplicationUser user in UserList)
            {
				var roleId = userRoles.FirstOrDefault(r => r.UserId == user.Id).RoleId;
				user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;

				if (user.Company == null)
				{
					user.Company = new Company()
					{
						Name = ""
					};
				}
			}

			return Json( new { users = UserList } );
        }

		[HttpPost]
		public IActionResult LockUnlock([FromBody]string id)
		{
			var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
			if (user == null)
			{
				return Json(new { success = true, message = "Error while Locking/Unlocking" });
			}

			if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
			{
				user.LockoutEnd = DateTime.Now;
				_context.SaveChanges();
				return Json(new { success = true, message = "This user are unlocked!" });
			}
			else
			{
				user.LockoutEnd = DateTime.Now.AddDays(30);
				_context.SaveChanges();
				return Json(new { success = true, message = "This user are locked for 30 days!" });
			}
		}

		[HttpDelete]
		public IActionResult Delete(string? id)
		{
			ApplicationUser userToBeDeleted = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);

			_context.ApplicationUsers.Remove(userToBeDeleted);
			_context.SaveChanges();

			return Json(new { success = true, message = "Delete successful!" });
		}
		#endregion
	}
}
