using System.Text.Json;
using WinterSheltonHouse.Models;

namespace WinterSheltonHouse.Services
{
    public class CartService
    {
        private const string CartKey = "Cart";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public List<CartItem> GetCart()
        {
            var json = Session.GetString(CartKey);
            return json == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(json)!;
        }

        public void SaveCart(List<CartItem> cart)
        {
            Session.SetString(CartKey, JsonSerializer.Serialize(cart));
        }

        public void AddItem(Product product, int qty = 1)
        {
            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.ProductId == product.Id);
            if (existing != null)
                existing.Quantity += qty;
            else
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = qty,
                    ImageUrl = product.ImageUrl
                });
            SaveCart(cart);
        }

        public void RemoveItem(int productId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.ProductId == productId);
            SaveCart(cart);
        }

        public void UpdateQuantity(int productId, int qty)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                if (qty <= 0) cart.Remove(item);
                else item.Quantity = qty;
            }
            SaveCart(cart);
        }

        public void ClearCart() => Session.Remove(CartKey);

        public int Count => GetCart().Sum(c => c.Quantity);
        public decimal Total => GetCart().Sum(c => c.Total);
    }
}
