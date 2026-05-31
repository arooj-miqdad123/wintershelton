using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterSheltonHouse.Data;
using WinterSheltonHouse.Models;

namespace WinterSheltonHouse.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalProducts = await _db.Products.CountAsync();
            ViewBag.TotalOrders = await _db.Orders.CountAsync();
            ViewBag.TotalRevenue = await _db.Orders.Where(o => o.Status != OrderStatus.Cancelled).SumAsync(o => o.TotalAmount);
            ViewBag.TotalCustomers = (await _userManager.GetUsersInRoleAsync("Customer")).Count;
            var recentOrders = await _db.Orders.Include(o => o.User).OrderByDescending(o => o.OrderDate).Take(10).ToListAsync();
            return View(recentOrders);
        }

        public async Task<IActionResult> Products()
        {
            return View(await _db.Products.OrderByDescending(p => p.Id).ToListAsync());
        }

        [HttpGet]
        public IActionResult CreateProduct() => View(new ProductEditViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Products.Add(new Product { Name = model.Name, Description = model.Description, Price = model.Price, Stock = model.Stock, Category = model.Category, ImageUrl = string.IsNullOrEmpty(model.ImageUrl) ? "/images/placeholder.jpg" : model.ImageUrl, IsActive = model.IsActive });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Product add ho gaya!";
            return RedirectToAction(nameof(Products));
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            return View(new ProductEditViewModel { Id = p.Id, Name = p.Name, Description = p.Description, Price = p.Price, Stock = p.Stock, Category = p.Category, ImageUrl = p.ImageUrl, IsActive = p.IsActive });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var p = await _db.Products.FindAsync(model.Id);
            if (p == null) return NotFound();
            p.Name = model.Name; p.Description = model.Description; p.Price = model.Price;
            p.Stock = model.Stock; p.Category = model.Category;
            p.ImageUrl = string.IsNullOrEmpty(model.ImageUrl) ? p.ImageUrl : model.ImageUrl;
            p.IsActive = model.IsActive;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Product update ho gaya!";
            return RedirectToAction(nameof(Products));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p != null) { _db.Products.Remove(p); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Product delete ho gaya!";
            return RedirectToAction(nameof(Products));
        }

        public async Task<IActionResult> Orders(string? status)
        {
            var query = _db.Orders.Include(o => o.User).AsQueryable();
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var st))
                query = query.Where(o => o.Status == st);
            ViewBag.Status = status;
            return View(await query.OrderByDescending(o => o.OrderDate).ToListAsync());
        }

        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _db.Orders.Include(o => o.User).Include(o => o.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            ViewBag.Order = order;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus status)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order != null) { order.Status = status; await _db.SaveChangesAsync(); TempData["Success"] = "Status update ho gaya!"; }
            return RedirectToAction(nameof(OrderDetail), new { id });
        }

        public async Task<IActionResult> Customers()
        {
            return View(await _userManager.GetUsersInRoleAsync("Customer"));
        }
    }
}
