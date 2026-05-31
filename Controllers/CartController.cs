using Microsoft.AspNetCore.Mvc;
using WinterSheltonHouse.Data;
using WinterSheltonHouse.Services;

namespace WinterSheltonHouse.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cart;
        private readonly AppDbContext _db;

        public CartController(CartService cart, AppDbContext db)
        {
            _cart = cart;
            _db = db;
        }

        public IActionResult Index()
        {
            var items = _cart.GetCart();
            ViewBag.Total = _cart.Total;
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int qty = 1)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null || !product.IsActive)
            {
                TempData["Error"] = "Product available nahi hai";
                return RedirectToAction("Products", "Home");
            }

            _cart.AddItem(product, qty);
            TempData["Success"] = $"{product.Name} cart mein add ho gaya!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            _cart.RemoveItem(productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(int productId, int qty)
        {
            _cart.UpdateQuantity(productId, qty);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Count()
        {
            return Json(new { count = _cart.Count });
        }
    }
}
