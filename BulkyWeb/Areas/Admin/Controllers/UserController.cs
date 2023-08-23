using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<IdentityUser> _userManager;

		public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		#region API CALL
		[HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<ApplicationUser> UserList = _unitOfWork.ApplicationUserRepository.GetAll(includeProperties: "Company");
            List<ApplicationUserVM> UserListWithRoles = new();

			foreach (ApplicationUser user in UserList)
            {
                IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
				string role = roles.FirstOrDefault();
				UserListWithRoles.Add(new()
				{
                    ApplicationUser = user,
                    Role = role,
				});
			}

			return Json( new { users = UserListWithRoles } );
        }

        #endregion
    }
}
