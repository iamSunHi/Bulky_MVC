using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }

		public OrderController(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

		public IActionResult Details(int orderId)
		{
            orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeaderRepository.Get(o => o.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetailRepository.GetAll(o => o.OrderHeaderId == orderId, includeProperties: "Product"),
            };
			return View(orderVM);
		}
        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetail()
		{
            var orderHeaderFromDb = _unitOfWork.OrderHeaderRepository.Get(o => o.Id == orderVM.OrderHeader.Id);
			orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
			orderHeaderFromDb.PhoneNumber =orderVM.OrderHeader.PhoneNumber;
			orderHeaderFromDb.Address =orderVM.OrderHeader.Address;
			orderHeaderFromDb.City =orderVM.OrderHeader.City;
			orderHeaderFromDb.State =orderVM.OrderHeader.State;
			orderHeaderFromDb.PostalCode =orderVM.OrderHeader.PostalCode;
			if (orderVM.OrderHeader.Carrier != null)
			{
				orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
			}
			if (orderVM.OrderHeader.TrackingNumber != null)
			{
				orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
			}

			_unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDb);
			_unitOfWork.Save();
			TempData["Success"] = "Order Details Updated Successfully.";

			return RedirectToAction("Details", "Order", new { orderId = orderHeaderFromDb.Id });
		}

		#region API CALL
		[HttpGet]
        public IActionResult GetAll([FromQuery]string status)
        {
			IEnumerable<OrderHeader> orderHeaders;

			if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
			{
				orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser");
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

				orderHeaders = _unitOfWork.OrderHeaderRepository.GetAll(o => o.ApplicationUserId == userId, includeProperties: "ApplicationUser");
			}

			switch (status)
			{
				case "inprocess":
					orderHeaders = orderHeaders.Where(o => o.OrderStatus == StaticDetails.StatusInProcess);
					break;
				case "pending":
					orderHeaders = orderHeaders.Where(o => o.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
					break;
				case "completed":
					orderHeaders = orderHeaders.Where(o => o.OrderStatus == StaticDetails.StatusShipped);
					break;
				case "approved":
					orderHeaders = orderHeaders.Where(o => o.OrderStatus == StaticDetails.StatusApproved);
					break;
				default:
					break;
			}

			return Json(new { data = orderHeaders });
        }

        //[HttpDelete]
        //public IActionResult Delete(int? id)
        //{
        //    var productToBeDeleted = _unitOfWork.ProductRepository.Get(p => p.Id == id);
        //    if (productToBeDeleted == null)
        //    {
        //        return Json(new { success = false, message = "Error while deleting!" });
        //    }

        //    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

        //    if (System.IO.File.Exists(oldImagePath))
        //    {
        //        System.IO.File.Delete(oldImagePath);
        //    }

        //    _unitOfWork.ProductRepository.Remove(productToBeDeleted);
        //    _unitOfWork.Save();

        //    return Json(new { success = true, message = "Delete successful!" });
        //}
        #endregion
    }
}
