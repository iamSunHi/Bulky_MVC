﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public string ISBN { get; set; } = null!;
		public string Author { get; set; } = null!;
        [ValidateNever]
        public string ImageUrl { get; set; } = null!;
        [Display(Name = "List Price"), Range(1, 1000)]

		public double ListPrice { get; set; }
		[Display(Name = "List Price 1-50"), Range(1, 1000)]
		public double Price { get; set; }
		[Display(Name = "List Price 50-100"), Range(1, 1000)]
		public double Price50 { get; set; }
		[Display(Name = "List Price 100+"), Range(1, 1000)]
		public double Price100 { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
		[ValidateNever]
		public Category Category { get; set; } = null!;

        [Display(Name = "Cover Type")]
        public int CoverTypeId { get; set; }
        [ValidateNever]
        public CoverType? CoverType { get; set; }
    }
}
