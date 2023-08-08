using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public ProductController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			IEnumerable<Product> ProductList = _unitOfWork.ProductRepository.GetAll();
			return View(ProductList);
		}

		[HttpPost]
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(Product product)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.ProductRepository.Add(product);
				_unitOfWork.Save();
				TempData["success"] = "Product created successfully!";
				return RedirectToAction("Index");
			}
			return View(product);
		}

		[HttpGet]
		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Product? product = _unitOfWork.ProductRepository.Get(c => c.Id == id);
			if (product == null)
			{
				return NotFound();
			}
			return View(product);
		}
		[HttpPost]
		public IActionResult Edit(Product product)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.ProductRepository.Update(product);
				_unitOfWork.Save();
				TempData["success"] = "Product updated successfully!";
				return RedirectToAction("Index");
			}
			return View(product);
		}

		[HttpGet]
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Product? product = _unitOfWork.ProductRepository.Get(c => c.Id == id);
			if (product == null)
			{
				return NotFound();
			}
			return View(product);
		}
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePOST(int? id)
		{
			Product? product = _unitOfWork.ProductRepository.Get(c => c.Id == id);
			if (product == null)
			{
				return NotFound();
			}
			_unitOfWork.ProductRepository.Remove(product);
			_unitOfWork.Save();
			TempData["success"] = "Product deleted successfully!";
			return RedirectToAction("Index");
		}
	}
}
