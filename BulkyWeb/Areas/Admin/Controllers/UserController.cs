using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

		public UserController(ApplicationDbContext context)
        {
			_context = context;
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
            ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == Id);
            return View(user);
		}
        [HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Update(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
				_context.ApplicationUsers.Update(user);
				_context.SaveChanges();
				/*var saved = false;
				while (!saved)
				{
					try
					{
						// Attempt to save changes to the database
						_context.SaveChanges();
						saved = true;
					}
					catch (DbUpdateConcurrencyException ex)
					{
						foreach (var entry in ex.Entries)
						{
							if (entry.Entity is ApplicationUser)
							{
								*//*var proposedValues = entry.CurrentValues;*//*
								var databaseValues = entry.GetDatabaseValues();

								*//*foreach (var property in proposedValues.Properties)
								{
									var proposedValue = proposedValues[property];
									var databaseValue = databaseValues[property];

									// TODO: decide which value should be written to database
									// proposedValues[property] = <value to be saved>;
								}*//*

								// Refresh original values to bypass next concurrency check
								entry.OriginalValues.SetValues(databaseValues);
							}
							else
							{
								throw new NotSupportedException(
									"Don't know how to handle concurrency conflicts for "
									+ entry.Metadata.Name);
							}
						}
					}
				}*/
				TempData["success"] = "User updated successfully!";
			    return RedirectToAction("Index");
			}
            return View(user);
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
