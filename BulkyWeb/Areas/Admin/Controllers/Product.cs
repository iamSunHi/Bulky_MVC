using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	public class Product : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
