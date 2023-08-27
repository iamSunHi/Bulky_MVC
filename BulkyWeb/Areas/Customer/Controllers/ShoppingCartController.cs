using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class ShoppingCartController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }

		public ShoppingCartController(ApplicationDbContext context, IUnitOfWork unitOfWork)
		{
			_context = context;
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new ShoppingCartVM()
			{
				ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new()
			};
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(ShoppingCartVM);
		}

		public IActionResult Summary()
		{
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);
			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

			ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
			string userRole = _context.Roles.FirstOrDefault(r => r.Id == roleId).Name;
			if (userRole == StaticDetails.Role_Company)
			{
				ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
				ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
			}
			else
			{
				ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;
				ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.StatusPending;
			}
			_unitOfWork.OrderHeaderRepository.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Save();

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count
				};
				_unitOfWork.OrderDetailRepository.Add(orderDetail);
			}
			_unitOfWork.Save();

			if (userRole != StaticDetails.Role_Company)
			{
				ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
				ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
			}

			return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
		}

		public IActionResult OrderConfirmation(int id)
		{
			return View(id);
		}

		public IActionResult Plus(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(c => c.Id == cartId);
			cartFromDb.Count++;
			_unitOfWork.ShoppingCartRepository.Update(cartFromDb);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}
		public IActionResult Minus(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(c => c.Id == cartId);
			if (cartFromDb.Count <= 1)
			{
				_unitOfWork.ShoppingCartRepository.Remove(cartFromDb);
			}
			else
			{
				cartFromDb.Count--;
				_unitOfWork.ShoppingCartRepository.Update(cartFromDb);
			}
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}
		public IActionResult Remove(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(c => c.Id == cartId);
			_unitOfWork.ShoppingCartRepository.Remove(cartFromDb);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 50)
			{
				return shoppingCart.Product.Price;
			}
			else if (shoppingCart.Count <= 100)
			{
				return shoppingCart.Product.Price50;
			}
			else
			{
				return shoppingCart.Product.Price100;
			}
		}
	}
}
