using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {
		private readonly ApplicationDbContext _context;
		[BindProperty]
		public Category Category { get; set; }

		public EditModel(ApplicationDbContext context)
		{
			_context = context;
		}

		public void OnGet(int Id)
        {
			Category = _context.Categories.FirstOrDefault(c => c.Id == Id);
		}

		public IActionResult OnPost()
		{
			_context.Categories.Update(Category);
			_context.SaveChanges();
			TempData["success"] = "Category updated successfully!";

			return RedirectToPage("Index");
		}
    }
}
