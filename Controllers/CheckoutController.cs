using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WinterSheltonHouse.Data;
using WinterSheltonHouse.Models;
using WinterSheltonHouse.Services;

namespace WinterSheltonHouse.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly CartService _cart;
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(CartService cart, AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _cart = cart;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var items = _cart.GetCart();
            if (!items.Any())
            {
                TempData["Error"] = "Aapka cart khali hai";
                return RedirectToAction("Index", "Cart");
            }

            var user = await _userManager.GetUserAsync(User);
            var model = new CheckoutViewModel
            {
                FullName = user?.FullName ?? "",
                CartItems = items,
                Total = _cart.Total
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var items = _cart.GetCart();
            if (!items.Any())
                return RedirectToAction("Index", "Cart");

            model.CartItems = items;
            model.Total = _cart.Total;

            // Card validation
            if (model.PaymentMethod == PaymentMethod.CreditCard)
            {
                if (string.IsNullOrEmpty(model.CardNumber) ||
                    string.IsNullOrEmpty(model.CardHolder) ||
                    string.IsNullOrEmpty(model.ExpiryDate) ||
                    string.IsNullOrEmpty(model.CVV))
                {
                    ModelState.AddModelError("", "Card ki tamam details bharein");
                }
                else
                {
                    // Fake card validation - accept any 16-digit card
                    var cleanCard = model.CardNumber.Replace(" ", "").Replace("-", "");
                    if (cleanCard.Length < 13 || cleanCard.Length > 19 || !cleanCard.All(char.IsDigit))
                    {
                        ModelState.AddModelError("CardNumber", "Valid card number enter karein");
                    }
                    if (model.CVV.Length < 3 || !model.CVV.All(char.IsDigit))
                    {
                        ModelState.AddModelError("CVV", "Valid CVV enter karein");
                    }
                }
            }

            if (!ModelState.IsValid)
                return View("Index", model);

            var user = await _userManager.GetUserAsync(User);

            var order = new Order
            {
                UserId = user!.Id,
                PaymentMethod = model.PaymentMethod,
                ShippingAddress = model.Address,
                ShippingCity = model.City,
                ShippingPhone = model.Phone,
                TotalAmount = _cart.Total,
                Status = OrderStatus.Pending,
                Items = items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.Price
                }).ToList()
            };

            if (model.PaymentMethod == PaymentMethod.CreditCard && model.CardNumber != null)
            {
                var clean = model.CardNumber.Replace(" ", "").Replace("-", "");
                order.CardLastFour = clean.Length >= 4 ? clean[^4..] : clean;
                order.Status = OrderStatus.Processing;
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            _cart.ClearCart();

            TempData["OrderId"] = order.Id;
            TempData["PaymentMethod"] = order.PaymentMethod.ToString();
            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            ViewBag.OrderId = TempData["OrderId"];
            ViewBag.PaymentMethod = TempData["PaymentMethod"];
            return View();
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = _db.Orders
                .Where(o => o.UserId == user!.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return View(orders);
        }

        public async Task<IActionResult> OrderDetail(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = _db.Orders
                .Where(o => o.Id == id && o.UserId == user!.Id)
                .Select(o => new
                {
                    o.Id, o.OrderDate, o.Status, o.PaymentMethod,
                    o.TotalAmount, o.ShippingAddress, o.ShippingCity,
                    o.ShippingPhone, o.CardLastFour,
                    Items = o.Items.Select(i => new
                    {
                        i.Quantity, i.UnitPrice,
                        ProductName = i.Product != null ? i.Product.Name : "Unknown"
                    })
                })
                .FirstOrDefault();

            if (order == null) return NotFound();
            return Json(order);
        }
    }
}
