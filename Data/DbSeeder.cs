using Microsoft.AspNetCore.Identity;
using WinterSheltonHouse.Models;

namespace WinterSheltonHouse.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.EnsureCreated();

            // Roles
            string[] roles = { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin User
            var adminEmail = "admin@wintershelton.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User",
                    EmailConfirmed = true,
                    Address = "Main Office, Winter Shelton House",
                    City = "Lahore"
                };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Test Customer
            var customerEmail = "customer@wintershelton.com";
            if (await userManager.FindByEmailAsync(customerEmail) == null)
            {
                var customer = new ApplicationUser
                {
                    UserName = customerEmail,
                    Email = customerEmail,
                    FullName = "Test Customer",
                    EmailConfirmed = true,
                    Address = "456 Test Street",
                    City = "Karachi"
                };
                var result = await userManager.CreateAsync(customer, "Customer@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(customer, "Customer");
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    // Sweaters
                    new Product { Name = "Classic Wool Sweater", Description = "Warm and comfortable pullover sweater crafted from pure wool — perfect for cold winter days.", Price = 2500, Stock = 50, Category = ProductCategory.Sweaters, ImageUrl = "https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=600&auto=format&fit=crop" },
                    new Product { Name = "Knitted Turtleneck", Description = "Premium knitted turtleneck sweater offering full neck coverage — ideal for harsh winters.", Price = 3200, Stock = 30, Category = ProductCategory.Sweaters, ImageUrl = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=600&auto=format&fit=crop" },
                    new Product { Name = "Cashmere Blend Sweater", Description = "The finest blend of cashmere and wool — ultra-soft to the touch and incredibly warm.", Price = 4500, Stock = 20, Category = ProductCategory.Sweaters, ImageUrl = "https://images.unsplash.com/photo-1576566588028-4147f3842f27?w=600&auto=format&fit=crop" },
                    new Product { Name = "Striped Winter Pullover", Description = "Stylish striped pullover with a relaxed fit — great for casual winter outings.", Price = 1800, Stock = 60, Category = ProductCategory.Sweaters, ImageUrl = "https://images.unsplash.com/photo-1556821840-3a63f15732ce?w=600&auto=format&fit=crop" },

                    // Jackets
                    new Product { Name = "Heavy Winter Jacket", Description = "Thick water-resistant jacket built to shield you from snowfall and biting cold.", Price = 6500, Stock = 25, Category = ProductCategory.Jackets, ImageUrl = "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=600&auto=format&fit=crop" },
                    new Product { Name = "Puffer Down Jacket", Description = "Lightweight yet incredibly warm puffer jacket — perfect for outdoor activities and travel.", Price = 5800, Stock = 35, Category = ProductCategory.Jackets, ImageUrl = "https://images.unsplash.com/photo-1544923246-77307dd654cb?w=600&auto=format&fit=crop" },
                    new Product { Name = "Leather Winter Coat", Description = "Stylish genuine leather coat that works equally well for formal and casual occasions.", Price = 9000, Stock = 15, Category = ProductCategory.Jackets, ImageUrl = "https://images.unsplash.com/photo-1548883354-94bcfe321cbb?w=600&auto=format&fit=crop" },
                    new Product { Name = "Fleece Zip Jacket", Description = "Soft fleece zip-up jacket with a comfortable fit — your everyday winter essential.", Price = 3500, Stock = 45, Category = ProductCategory.Jackets, ImageUrl = "https://images.unsplash.com/photo-1539533113208-f6df8cc8b543?w=600&auto=format&fit=crop" },

                    // Gloves
                    new Product { Name = "Woolen Winter Gloves", Description = "Pure wool gloves that keep your hands cozy and warm throughout the coldest months.", Price = 500, Stock = 100, Category = ProductCategory.Gloves, ImageUrl = "https://images.unsplash.com/photo-1601924994987-69e26d50dc26?w=600&auto=format&fit=crop" },
                    new Product { Name = "Leather Touch Screen Gloves", Description = "Sleek leather gloves with touch-screen compatible fingertips — stay connected in the cold.", Price = 1200, Stock = 80, Category = ProductCategory.Gloves, ImageUrl = "https://images.unsplash.com/photo-1585386959984-a4155224a1ad?w=600&auto=format&fit=crop" },
                    new Product { Name = "Thermal Ski Gloves", Description = "Specially designed thermal gloves for skiing and outdoor winter sports.", Price = 1800, Stock = 40, Category = ProductCategory.Gloves, ImageUrl = "https://images.unsplash.com/photo-1551698618-1dfe5d97d256?w=600&auto=format&fit=crop" },
                    new Product { Name = "Kids Winter Mittens", Description = "Warm and colorful mittens designed for children — keeping little hands toasty all winter.", Price = 350, Stock = 120, Category = ProductCategory.Gloves, ImageUrl = "https://images.unsplash.com/photo-1608744882201-52a7f7f3dd60?w=600&auto=format&fit=crop" },

                    // Socks
                    new Product { Name = "Thermal Wool Socks", Description = "Thick wool socks with anti-slip sole — keep your feet warm from morning to night.", Price = 300, Stock = 200, Category = ProductCategory.Socks, ImageUrl = "https://images.unsplash.com/photo-1586350977771-b3b0abd50c82?w=600&auto=format&fit=crop" },
                    new Product { Name = "Cashmere Socks Pair", Description = "Ultra-soft cashmere socks with a luxurious feel — the perfect winter gift.", Price = 750, Stock = 150, Category = ProductCategory.Socks, ImageUrl = "https://images.unsplash.com/photo-1617325247661-675ab4b64ae2?w=600&auto=format&fit=crop" },
                    new Product { Name = "Hiking Winter Socks", Description = "Durable thick socks engineered for trekking and hiking in cold conditions.", Price = 450, Stock = 180, Category = ProductCategory.Socks, ImageUrl = "https://images.unsplash.com/photo-1607082349566-187342175e2f?w=600&auto=format&fit=crop" },
                    new Product { Name = "Kids Colorful Socks Pack", Description = "A pack of 5 pairs of bright, colorful socks for kids — fun and warm!", Price = 600, Stock = 250, Category = ProductCategory.Socks, ImageUrl = "https://images.unsplash.com/photo-1582791694770-cbdc9dda338f?w=600&auto=format&fit=crop" },
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
