using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class DeleteModel : PageModel
    {
		private readonly ApplicationDbContext _context;
		[BindProperty]
		public Category Category { get; set; }

		public DeleteModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public void OnGet(int Id)
		{
			Category = _context.Categories.FirstOrDefault(c => c.Id == Id);
		}

		public IActionResult OnPost()
		{
			if (ModelState.IsValid)
			{
				_context.Categories.Remove(Category);
				_context.SaveChanges();
				TempData["success"] = "Category deleted successfully!";
				return RedirectToPage("Index");
			}
			return Page();
		}
	}
}
