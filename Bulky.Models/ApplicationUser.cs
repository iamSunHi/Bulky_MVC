﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class ApplicationUser : IdentityUser
	{
		public string Name { get; set; } = null!;
		public string? Address { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
		public string? PostalCode { get; set; }

        public int? CompanyId { get; set; }
        [ValidateNever]
        public Company? Company { get; set; }

		[NotMapped]
		public string Role { get; set; } = null!;
	}
}
