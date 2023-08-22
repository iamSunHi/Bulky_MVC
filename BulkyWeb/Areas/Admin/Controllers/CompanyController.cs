using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = StaticDetails.Role_Admin)]
	/*[Authorize(Roles = StaticDetails.Role_Company)]*/
	public class CompanyController : Controller
    {
		private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

		public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Company> CompanyList = _unitOfWork.CompanyRepository.GetAll();
            return Json( new { companies = CompanyList } );
		}

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company CompanyToDelete = _unitOfWork.CompanyRepository.Get(c => c.Id == id);
			if (CompanyToDelete == null)
			{
				return Json(new { success = false, message = "Error while deleting!" });
			}

            _unitOfWork.CompanyRepository.Remove(CompanyToDelete);
            _unitOfWork.Save();

			return Json(new { success = true, message = "Delete successful!" });
		}
		#endregion
	}
}
