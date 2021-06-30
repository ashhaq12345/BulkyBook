using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(IncludeProperties: "Category,CoverType");
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart
                    .GetAll(x => x.ApplicationUserId == claim.Value)
                    .ToList().Count();

                HttpContext.Session.SetInt32(SD.SSShopingCart, count);
            }

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product
                .GetFirstOrDefault(x => x.Id == id, IncludeProperties: "Category,CoverType");

            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cartObj)
        {
            cartObj.Id = 0;
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObj.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.ApplicationUserId == cartObj.ApplicationUserId && u.ProductId == cartObj.ProductId);

                if (cartFromDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(cartObj);
                }
                else
                {
                    cartFromDb.Count += cartObj.Count;
                }
                _unitOfWork.Save();
                var count = _unitOfWork.ShoppingCart
                    .GetAll(x => x.ApplicationUserId == cartObj.ApplicationUserId)
                    .ToList().Count();

                HttpContext.Session.SetInt32(SD.SSShopingCart, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Details(cartObj.ProductId);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
