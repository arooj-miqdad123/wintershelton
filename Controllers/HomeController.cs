using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterSheltonHouse.Data;
using WinterSheltonHouse.Models;

namespace WinterSheltonHouse.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var featured = await _db.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Id)
                .Take(8)
                .ToListAsync();
            return View(featured);
        }

        public async Task<IActionResult> Products(string? category, string? search, string? sort)
        {
            var query = _db.Products.Where(p => p.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(category) && Enum.TryParse<ProductCategory>(category, out var cat))
                query = query.Where(p => p.Category == cat);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));

            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            ViewBag.Category = category;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Privacy() => View();
        public IActionResult About() => View();
    }
}
