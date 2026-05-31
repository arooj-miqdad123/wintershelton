using System.ComponentModel.DataAnnotations;

namespace WinterSheltonHouse.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Total => Price * Quantity;
    }

    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Naam zaroori hai")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone zaroori hai")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address zaroori hai")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sheher ka naam zaroori hai")]
        public string City { get; set; } = string.Empty;

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        // Card fields (only if card payment)
        public string? CardNumber { get; set; }
        public string? CardHolder { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }

        public List<CartItem> CartItems { get; set; } = new();
        public decimal Total { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email zaroori hai")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password zaroori hai")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Poora naam zaroori hai")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zaroori hai")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password zaroori hai")]
        [MinLength(6, ErrorMessage = "Password kam az kam 6 characters ka hona chahiye")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Passwords match nahi hote")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ProductEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 999999)]
        public decimal Price { get; set; }

        [Range(0, 9999)]
        public int Stock { get; set; }

        [Required]
        public ProductCategory Category { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
