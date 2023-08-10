using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			IEnumerable<Product> ProductList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category");
			return View(ProductList);
		}
		
		public IActionResult Upsert(int? id)
		{
			ProductVM productVM = new()
			{
				Product = new Product(),
				CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(c => new SelectListItem
				{
					Text = c.Name,
					Value = c.Id.ToString(),
				}),
			};
			if (id == null || id == 0)
				return View(productVM);
			else
			{
				productVM.Product = _unitOfWork.ProductRepository.Get(p => p.Id == id);
				return View(productVM);
			}
		}
		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\product");

					if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
					{
						var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					productVM.Product.ImageUrl = @"\images\product\" + fileName;
				}

				if (productVM.Product.Id == 0)
				{
					_unitOfWork.ProductRepository.Add(productVM.Product);
					TempData["success"] = "Product created successfully!";
				}
				else
				{
					_unitOfWork.ProductRepository.Update(productVM.Product);
					TempData["success"] = "Product updated successfully!";
				}

				_unitOfWork.Save();
				return RedirectToAction("Index");
			}
			else
			{
				productVM.CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(c => new SelectListItem
				{
					Text = c.Name,
					Value = c.Id.ToString(),
				});
				return View(productVM);
			}
		}

		#region API CALL
		[HttpGet]
		public IActionResult GetAll()
		{
			IEnumerable<Product> ProductList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category");
			return Json( new { products = ProductList } );
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitOfWork.ProductRepository.Get(p => p.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting!" });
			}

			var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}

			_unitOfWork.ProductRepository.Remove(productToBeDeleted);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Delete successfull!" });
		}
		#endregion
	}
}
