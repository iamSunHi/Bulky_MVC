using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
	public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
        
		public CategoryController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        
		public IActionResult Index()
        {
            IEnumerable<Category> CategoryList = _categoryRepo.GetAll();
            return View(CategoryList);
        }

        [HttpGet]
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Add(category);
                _categoryRepo.Save();
				TempData["success"] = "Category created successfully!";
                return RedirectToAction("Index");
            }
            return View(category);
        }

		[HttpGet]
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _categoryRepo.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
		}
		[HttpPost]
		public IActionResult Edit(Category category)
		{
			if (ModelState.IsValid)
			{
				_categoryRepo.Update(category);
				_categoryRepo.Save();
				TempData["success"] = "Category updated successfully!";
				return RedirectToAction("Index");
			}
			return View(category);
		}

		[HttpGet]
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Category? category = _categoryRepo.Get(c => c.Id == id);
			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePOST(int? id)
		{
			Category? category = _categoryRepo.Get(c => c.Id == id);
			if (category == null)
			{
				return NotFound();
			}
			_categoryRepo.Remove(category);
			_categoryRepo.Save();
			TempData["success"] = "Category deleted successfully!";
			return RedirectToAction("Index");
		}
	}
}
