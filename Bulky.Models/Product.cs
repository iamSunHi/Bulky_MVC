using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public string ISBN { get; set; } = null!;
		public string Author { get; set; } = null!;
		[Display(Name = "List Price"), Range(1, 1000)]
		public double ListPrice { get; set; }
		[Display(Name = "List Price 1-50"), Range(1, 1000)]
		public double Price { get; set; }
		[Display(Name = "List Price 50-100"), Range(1, 1000)]
		public double Price50 { get; set; }
		[Display(Name = "List Price 100+"), Range(1, 1000)]
		public double Price100 { get; set; }
	}
}
