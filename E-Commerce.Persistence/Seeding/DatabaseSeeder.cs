using System.Reflection;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Domain.Common;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Persistence.Seeding;

public sealed class DatabaseSeeder
{
    private const string SeedSlug = "seed-electronics";
    private const string Currency = "EGP";

    private static readonly string[] RootCategories =
    [
        "Seed Electronics",
        "Seed Home Living",
        "Seed Fashion",
        "Seed Sports Gear",
        "Seed Beauty Care",
        "Seed Books Media"
    ];

    private static readonly string[] SubCategories =
    [
        "Smart Phones",
        "Laptop Computers",
        "Audio Accessories",
        "Kitchen Appliances",
        "Office Furniture",
        "Men Apparel",
        "Women Apparel",
        "Fitness Equipment",
        "Skin Care",
        "Business Books",
        "Gaming Accessories",
        "Travel Essentials"
    ];

    private static readonly string[] Brands =
    [
        "NileTech",
        "CairoCraft",
        "DeltaHome",
        "AlexWear",
        "UrbanFit",
        "BrightBox",
        "SaharaStyle",
        "MetroGoods"
    ];

    private static readonly string[] ProductNouns =
    [
        "Wireless Headphones",
        "Smart Watch",
        "Cotton Hoodie",
        "Ergonomic Chair",
        "Coffee Maker",
        "Running Shoes",
        "Laptop Backpack",
        "Desk Lamp",
        "Yoga Mat",
        "Bluetooth Speaker",
        "Air Fryer",
        "Business Notebook",
        "Gaming Mouse",
        "Skin Serum",
        "Travel Organizer"
    ];

    private static readonly string[] Colors =
    [
        "Black",
        "White",
        "Navy",
        "Olive",
        "Silver",
        "Graphite",
        "Rose",
        "Teal"
    ];

    private static readonly string[] ColorHexCodes =
    [
        "#000000",
        "#FFFFFF",
        "#000080",
        "#808000",
        "#C0C0C0",
        "#41424C",
        "#FF007F",
        "#008080"
    ];

    private static readonly string[] Sizes = ["XS", "S", "M", "L", "XL", "One Size"];

    private static readonly string[] FirstNames =
    [
        "Omar",
        "Mariam",
        "Youssef",
        "Nour",
        "Salma",
        "Karim",
        "Hana",
        "Ali",
        "Laila",
        "Mona",
        "Ahmed",
        "Farida",
        "Tarek",
        "Dina",
        "Mostafa",
        "Yara"
    ];

    private static readonly string[] LastNames =
    [
        "Hassan",
        "Ibrahim",
        "Mahmoud",
        "Saleh",
        "Fouad",
        "Nabil",
        "Samir",
        "Adel",
        "Kamal",
        "Rashad"
    ];

    private static readonly string[] CairoAreas =
    [
        "Nasr City",
        "Heliopolis",
        "Maadi",
        "Zamalek",
        "New Cairo",
        "Dokki",
        "Mohandessin",
        "Madinaty"
    ];

    private readonly EcommerceContext _context;
    private readonly IPasswordHasherAdapter _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly Random _random = new(20260520);

    public DatabaseSeeder(
        EcommerceContext context,
        IPasswordHasherAdapter passwordHasher,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedDataAsync(CancellationToken cancellationToken = default)
    {
        var alreadySeeded = await _context.Categories
            .AsNoTracking()
            .AnyAsync(x => x.Slug == Slug.Create(SeedSlug), cancellationToken);

        if (alreadySeeded)
        {
            _logger.LogInformation("Development seed data already exists");
            return;
        }

        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            _logger.LogInformation("Development seed data started");

            var now = DateTimeOffset.UtcNow;
            var currency = CurrencyCode.Create(Currency);

            var categories = await SeedCategoriesAsync(cancellationToken);
            var users = await SeedUsersAsync(now, cancellationToken);
            var products = await SeedCatalogAsync(categories, currency, now, cancellationToken);
            var variants = products.SelectMany(x => x.Variants).ToList();

            await SeedInventoryAsync(variants, users, now, cancellationToken);
            await SeedCustomerActivityAsync(users, variants, now, cancellationToken);
            await SeedOrdersAsync(users, variants, currency, now, cancellationToken);
            await SeedOperationalRecordsAsync(users, now, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Development seed data completed with {UserCount} users, {CategoryCount} categories, {ProductCount} products and {VariantCount} variants",
                users.Count,
                categories.Count,
                products.Count,
                variants.Count);
        });
    }

    private async Task<List<Category>> SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = new List<Category>();

        for (var i = 0; i < RootCategories.Length; i++)
        {
            var root = Category.Create(
                null,
                RootCategories[i],
                Slug.Create(ToSlug(RootCategories[i])),
                i + 1);

            Stamp(root, MonthsAgo(10).AddDays(i));
            categories.Add(root);
        }

        for (var i = 0; i < SubCategories.Length; i++)
        {
            var parent = categories[i % RootCategories.Length];
            var child = Category.Create(
                parent.Id,
                $"Seed {SubCategories[i]}",
                Slug.Create(ToSlug($"Seed {SubCategories[i]}")),
                i + 1);

            Stamp(child, MonthsAgo(9).AddDays(i));
            categories.Add(child);
        }

        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync(cancellationToken);

        return categories;
    }

    private async Task<List<User>> SeedUsersAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        var passwordHash = _passwordHasher.Hash("DevSeed!2026");
        var users = new List<User>();
        var profiles = new List<UserProfile>();
        var addresses = new List<UserAddress>();

        for (var i = 0; i < 48; i++)
        {
            var firstName = FirstNames[i % FirstNames.Length];
            var lastName = LastNames[(i * 3) % LastNames.Length];
            var role = i == 0 ? UserRole.Admin : UserRole.Customer;
            var user = User.Create(
                $"seed.user{i + 1:000}@example.test",
                passwordHash,
                firstName,
                lastName,
                $"+2010{1000000 + i:0000000}",
                role);

            if (i == 0 || i % 8 != 0)
                user.Verify();

            if (i % 17 == 0 && i != 0)
                user.Deactivate();

            Stamp(user, MonthsAgo(8).AddDays(i));
            users.Add(user);

            var profile = UserProfile.Create(user.Id, firstName, lastName);
            profile.Update(
                firstName,
                lastName,
                null,
                i % 2 == 0 ? Gender.Male : Gender.Female,
                DateOnly.FromDateTime(now.AddYears(-22 - (i % 23)).Date),
                $"https://images.example.test/avatars/seed-user-{i + 1:000}.webp",
                now.AddDays(-(i % 30)));
            Stamp(profile, user.CreatedAt.AddHours(2));
            profiles.Add(profile);

            var addressCount = i % 4 == 0 ? 2 : 1;
            for (var addressIndex = 0; addressIndex < addressCount; addressIndex++)
            {
                var address = CreateAddress(
                    user.Id,
                    addressIndex == 0 ? AddressLabel.Home : AddressLabel.Work,
                    CairoAreas[(i + addressIndex) % CairoAreas.Length],
                    addressIndex == 0);
                Stamp(address, user.CreatedAt.AddDays(addressIndex + 1));
                addresses.Add(address);
            }
        }

        _context.Users.AddRange(users);
        _context.UserProfiles.AddRange(profiles);
        _context.UserAddresses.AddRange(addresses);
        await _context.SaveChangesAsync(cancellationToken);

        return users;
    }

    private async Task<List<Product>> SeedCatalogAsync(
        IReadOnlyList<Category> categories,
        CurrencyCode currency,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var products = new List<Product>();
        var leafCategories = categories.Where(x => x.ParentId is not null).ToList();

        for (var i = 0; i < 120; i++)
        {
            var category = leafCategories[i % leafCategories.Count];
            var name = BuildProductName(i);
            var price = Money.Create(RoundMoney(150 + _random.Next(0, 9500) + _random.Next(0, 99) / 100m), currency);
            var status = i % 23 == 0
                ? ProductStatus.Archived
                : i % 11 == 0
                    ? ProductStatus.OutOfStock
                    : ProductStatus.Active;

            var product = Product.Create(
                name,
                category.Id,
                Slug.Create(ToSlug($"{name}-{i + 1:000}")),
                price,
                hasVariants: true,
                Brands[i % Brands.Length],
                status);

            if (status is ProductStatus.Archived)
                product.Deactivate(now.AddDays(-(i % 40)));

            Stamp(product, MonthsAgo(7).AddDays(i));

            var imageCount = i % 5 == 0 ? 3 : 2;
            for (var imageIndex = 0; imageIndex < imageCount; imageIndex++)
            {
                var image = ProductImage.CreatePending(
                    product.Id,
                    $"seed/products/{product.Id:N}/image-{imageIndex + 1}.webp",
                    imageIndex == 0,
                    imageIndex + 1,
                    now.AddDays(30));
                image.MarkUploaded(
                    $"https://images.example.test/products/{product.Id:N}/{imageIndex + 1}.webp",
                    1200,
                    1200,
                    180_000 + imageIndex * 25_000,
                    "webp");
                Stamp(image, product.CreatedAt.AddHours(imageIndex + 1));
                product.AddImage(image, product.CreatedAt.AddHours(imageIndex + 2));
            }

            var variantCount = i % 4 == 0 ? 4 : 3;
            for (var variantIndex = 0; variantIndex < variantCount; variantIndex++)
            {
                var variantPriceOverride = variantIndex == 0
                    ? null
                    : Money.Create(price.Amount + variantIndex * 75, currency);

                var variant = product.AddVariant(
                    $"SEED-{i + 1:0000}-{variantIndex + 1:00}",
                    Sizes[(i + variantIndex) % Sizes.Length],
                    GetSeedColor(i + variantIndex * 2),
                    variantPriceOverride,
                    variantIndex == 0,
                    product.CreatedAt.AddHours(variantIndex + 4));

                Stamp(variant, product.CreatedAt.AddHours(variantIndex + 4));
                SetPrivateProperty(variant, nameof(Variant.Product), product);

                if (variantIndex == 0 || i % 6 == 0)
                {
                    var image = VariantImage.CreatePending(
                        variant.Id,
                        $"seed/variants/{variant.Id:N}/image-1.webp",
                        true,
                        1,
                        now.AddDays(30));
                    image.MarkUploaded(
                        $"https://images.example.test/variants/{variant.Id:N}/1.webp",
                        1000,
                        1000,
                        135_000,
                        "webp");
                    Stamp(image, variant.CreatedAt.AddHours(1));
                    variant.AddImage(image);
                }
            }

            products.Add(product);
        }

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync(cancellationToken);

        return products;
    }

    private async Task SeedInventoryAsync(
        IReadOnlyList<Variant> variants,
        IReadOnlyList<User> users,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var adminUserId = users.First(x => x.Role == UserRole.Admin).Id;
        var inventories = new List<Inventory>();
        var movements = new List<StockMovement>();

        for (var i = 0; i < variants.Count; i++)
        {
            var onHand = i % 17 == 0 ? 0 : _random.Next(5, 180);
            var variant = variants[i];
            var inventory = Inventory.Create(variant.Id, onHand, now.AddDays(-(i % 60)));
            Stamp(inventory, variant.CreatedAt.AddHours(2));
            inventories.Add(inventory);

            var initial = StockMovement.Create(
                variant.Id,
                StockMovementType.InitialStock,
                Math.Max(onHand, 1),
                "Initial development seed stock",
                null,
                adminUserId,
                inventory.CreatedAt);
            Stamp(initial, inventory.CreatedAt);
            movements.Add(initial);

            if (onHand > 20 && i % 4 == 0)
            {
                var sale = StockMovement.Create(
                    variant.Id,
                    StockMovementType.Sale,
                    -_random.Next(1, 5),
                    "Seeded fulfilled order stock movement",
                    null,
                    adminUserId,
                    now.AddDays(-(i % 30)));
                Stamp(sale, sale.UpdatedAt);
                movements.Add(sale);
            }
        }

        _context.Inventories.AddRange(inventories);
        _context.StockMovements.AddRange(movements);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedCustomerActivityAsync(
        IReadOnlyList<User> users,
        IReadOnlyList<Variant> variants,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var favorites = new List<Favorite>();
        var carts = new List<Cart>();
        var notifications = new List<Notification>();
        var stockAlerts = new List<StockAlert>();
        var favoriteKeys = new HashSet<string>();
        var stockAlertKeys = new HashSet<string>();

        for (var i = 1; i < users.Count; i++)
        {
            var user = users[i];
            var favoriteCount = 1 + i % 4;

            for (var j = 0; j < favoriteCount; j++)
            {
                var variant = variants[(i * 7 + j * 11) % variants.Count];
                var key = $"{user.Id:N}:{variant.Id:N}";
                if (favoriteKeys.Add(key))
                {
                    favorites.Add(new Favorite
                    {
                        UserId = user.Id,
                        VariantId = variant.Id,
                        CreatedAt = now.AddDays(-(i + j) % 90)
                    });
                }
            }

            if (i % 2 == 0)
            {
                var cart = Cart.CreateForUser(user.Id, now.AddDays(-(i % 20)));
                Stamp(cart, now.AddDays(-(i % 20)));
                for (var j = 0; j < 1 + i % 3; j++)
                {
                    var variant = variants[(i * 5 + j * 9) % variants.Count];
                    var item = CartItem.Create(cart.Id, variant.Id, 1 + j, cart.CreatedAt.AddHours(j + 1));
                    Stamp(item, cart.CreatedAt.AddHours(j + 1));
                    cart.AddItem(item, item.CreatedAt);
                }

                if (i % 10 == 0)
                    cart.SetStatus(CartStatus.Abandoned, now.AddDays(-(i % 7)));

                carts.Add(cart);
            }

            for (var j = 0; j < 2; j++)
            {
                notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Type = j == 0 ? "OrderStatusChanged" : "Promotion",
                    Title = j == 0 ? "Your order status changed" : "New offers available",
                    Body = j == 0
                        ? "A recent order has a new status in your account."
                        : "Selected products from your favorite categories are discounted.",
                    DataJson = JsonText.From(new { seed = true, userId = user.Id }).Value,
                    IsRead = (i + j) % 3 == 0,
                    ReadAt = (i + j) % 3 == 0 ? now.AddDays(-(i % 14)) : null,
                    CreatedAt = now.AddDays(-(i + j) % 45)
                });
            }

            if (i % 5 == 0)
            {
                var variant = variants[(i * 13) % variants.Count];
                var key = $"{user.Id:N}:{variant.Id:N}";
                if (stockAlertKeys.Add(key))
                {
                    stockAlerts.Add(new StockAlert
                    {
                        UserId = user.Id,
                        VariantId = variant.Id,
                        Status = i % 10 == 0 ? StockAlertStatus.Triggered : StockAlertStatus.Active,
                        TriggeredAt = i % 10 == 0 ? now.AddDays(-(i % 12)) : null,
                        CreatedAt = now.AddDays(-(i % 50))
                    });
                }
            }
        }

        _context.Favorites.AddRange(favorites);
        _context.Carts.AddRange(carts);
        _context.Notifications.AddRange(notifications);
        _context.StockAlerts.AddRange(stockAlerts);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedOrdersAsync(
        IReadOnlyList<User> users,
        IReadOnlyList<Variant> variants,
        CurrencyCode currency,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var orders = new List<Order>();
        var attempts = new List<PaymentAttempt>();

        for (var i = 0; i < 220; i++)
        {
            var user = users[1 + i % (users.Count - 1)];
            var orderDate = now.AddDays(-_random.Next(1, 180)).AddMinutes(i);
            var order = Order.Create(
                user.Id,
                $"SEED-{orderDate:yyyyMMdd}-{i + 1:000000}",
                currency,
                JsonText.From(CreateShippingAddress(user, i)),
                null,
                i % 13 == 0 ? "Seed order with customer note" : null,
                orderDate);
            Stamp(order, orderDate);

            var itemCount = 1 + i % 4;
            for (var j = 0; j < itemCount; j++)
            {
                var variant = variants[(i * 17 + j * 23) % variants.Count];
                var unitPrice = variant.GetPrice().Amount;
                var quantity = 1 + (i + j) % 3;
                var item = OrderItem.Create(
                    order.Id,
                    variant.Id,
                    currency,
                    variant.Sku,
                    variant.Product.Name,
                    JsonText.From(new { variant.Size, Color = variant.Color.Name }),
                    unitPrice,
                    quantity);

                Stamp(item, orderDate.AddMinutes(j + 2));
                order.AddItem(item, item.CreatedAt);
            }

            order.SetShippingFee(35 + i % 5 * 10, orderDate.AddMinutes(8));
            if (i % 9 == 0)
                order.ApplyDiscount(25, orderDate.AddMinutes(9));
            if (i % 6 == 0)
                order.SetTaxTotal(decimal.Round(order.Subtotal * 0.14m, 2), orderDate.AddMinutes(10));

            var status = PickOrderStatus(i);
            if (status == OrderStatus.Cancelled)
            {
                order.Cancel("Customer cancelled before fulfillment", orderDate.AddHours(2));
            }
            else if (status != OrderStatus.Pending)
            {
                order.MarkPaid(orderDate.AddHours(1));
                if (status != OrderStatus.Paid)
                    SetPrivateProperty(order, nameof(Order.Status), status);
            }

            if (status != OrderStatus.Pending && status != OrderStatus.Cancelled)
            {
                var payment = Payment.Create(
                    order.Id,
                    "paymob",
                    order.GrandTotal,
                    currency,
                    JsonText.From(new { provider = "paymob", seed = true }),
                    orderDate.AddMinutes(15));
                payment.AttachProviderPaymentId($"seed-pay-{i + 1:000000}", orderDate.AddMinutes(16));

                if (status == OrderStatus.PaymentFailed)
                    payment.MarkFailed(orderDate.AddMinutes(17), JsonText.From(new { reason = "Seed payment failure" }));
                else
                    payment.MarkCaptured(orderDate.AddMinutes(17));

                if (status is OrderStatus.Refunded or OrderStatus.PartiallyRefunded)
                {
                    var refund = payment.RequestRefund(
                        JsonText.From(new { seed = true }),
                        orderDate.AddDays(3),
                        status == OrderStatus.Refunded ? "Full customer refund" : "Partial customer refund");
                    refund.SetProviderRefundId($"seed-ref-{i + 1:000000}");
                    if (i % 4 == 0)
                        refund.MarkAsSucceeded();
                }

                order.AddPayment(payment, orderDate.AddMinutes(18));
            }

            var attempt = PaymentAttempt.Create(
                order.Id,
                "paymob",
                Math.Max(order.GrandTotal, 1),
                currency,
                $"seed-idempotency-{i + 1:000000}",
                orderDate.AddMinutes(45),
                orderDate.AddMinutes(12),
                $"seed-request-hash-{i + 1:000000}");
            attempt.AttachProviderSession(
                $"seed-session-{i + 1:000000}",
                $"https://checkout.example.test/seed-session-{i + 1:000000}",
                orderDate.AddMinutes(13),
                JsonText.From(new { seed = true, orderId = order.Id }));

            if (status == OrderStatus.PaymentFailed)
                attempt.MarkFailed(orderDate.AddMinutes(20), JsonText.From(new { reason = "Seed decline" }));
            else if (status != OrderStatus.Pending && status != OrderStatus.Cancelled)
                attempt.MarkPaid(orderDate.AddMinutes(20), JsonText.From(new { paid = true }));

            Stamp(attempt, orderDate.AddMinutes(12));
            attempts.Add(attempt);
            orders.Add(order);
        }

        _context.Orders.AddRange(orders);
        _context.PaymentAttempts.AddRange(attempts);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedOperationalRecordsAsync(
        IReadOnlyList<User> users,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var admin = users.First(x => x.Role == UserRole.Admin);
        var auditLogs = new List<AuditLog>();
        var authTokens = new List<AuthToken>();
        var refreshTokens = new List<RefreshToken>();
        var emailMessages = new List<EmailMessage>();

        for (var i = 0; i < 30; i++)
        {
            var user = users[i % users.Count];
            var createdAt = now.AddDays(-(i % 45));

            auditLogs.Add(new AuditLog
            {
                ActorUserId = admin.Id,
                Action = i % 3 == 0 ? AuditAction.Update : AuditAction.Create,
                EntityType = i % 2 == 0 ? "Product" : "Order",
                EntityId = Guid.NewGuid().ToString("N"),
                OldValuesJson = JsonText.Create("{}"),
                NewValuesJson = JsonText.From(new { seed = true, index = i }),
                IpAddress = $"10.0.0.{10 + i}",
                UserAgent = "SeedDataGenerator/1.0",
                CreatedAt = createdAt
            });

            var authToken = AuthToken.Create(
                user.Id,
                i % 2 == 0 ? TokenType.VerifyEmailToken : TokenType.ResetPasswordToken,
                TokenHash.Create($"seed-auth-token-hash-{i + 1:00000000000000000000}"),
                now.AddDays(7));
            Stamp(authToken, createdAt);
            if (i % 5 == 0)
                authToken.Consume();
            authTokens.Add(authToken);

            var refreshToken = RefreshToken.Create(
                user.Id,
                TokenHash.Create($"seed-refresh-token-hash-{i + 1:00000000000000000000}"),
                now.AddDays(30),
                createdAt,
                i % 2 == 0 ? "Chrome on Windows" : "Mobile Safari",
                $"192.0.2.{i + 1}");
            Stamp(refreshToken, createdAt);
            if (i % 6 == 0)
                refreshToken.Revoke(createdAt.AddDays(3));
            refreshTokens.Add(refreshToken);

            var email = EmailMessage.Create(
                user.Id,
                authToken.Id,
                i % 2 == 0 ? MessageType.VerifyEmailMessage : MessageType.ResetPasswordMessage,
                user.Email,
                i % 2 == 0 ? "Verify your seed account" : "Reset your seed password",
                "<p>This is a development seed email.</p>");
            Stamp(email, createdAt);
            if (i % 3 == 0)
                email.MarkAsSent("SeedProvider");
            else if (i % 7 == 0)
                email.MarkAsFailed("Seeded delivery failure");
            emailMessages.Add(email);

        }

        _context.AuditLogs.AddRange(auditLogs);
        _context.AuthTokens.AddRange(authTokens);
        _context.RefreshTokens.AddRange(refreshTokens);
        _context.EmailMessages.AddRange(emailMessages);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static UserAddress CreateAddress(
        Guid userId,
        AddressLabel label,
        string area,
        bool isDefault)
    {
        return UserAddress.Create(
            userId,
            label,
            "Egypt",
            "Cairo",
            "Cairo",
            area,
            $"{12 + area.Length} {area} Street",
            $"{10 + area.Length}",
            label == AddressLabel.Work ? "4" : null,
            label == AddressLabel.Home ? "12" : null,
            "11835",
            "Near main square",
            30.0444m + area.Length / 10000m,
            31.2357m + area.Length / 10000m,
            isDefault);
    }

    private static ShippingAddressDto CreateShippingAddress(User user, int index)
    {
        var area = CairoAreas[index % CairoAreas.Length];
        return new ShippingAddressDto(
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.Phone ?? "+201010000000",
            "Cairo",
            $"{22 + index % 70} {area} Street",
            $"Apartment {1 + index % 20}");
    }

    private static OrderStatus PickOrderStatus(int index)
    {
        return (index % 20) switch
        {
            0 => OrderStatus.Pending,
            1 => OrderStatus.PaymentFailed,
            2 => OrderStatus.Cancelled,
            3 => OrderStatus.Processing,
            4 => OrderStatus.Shipped,
            5 => OrderStatus.Delivered,
            6 => OrderStatus.Refunded,
            7 => OrderStatus.PartiallyRefunded,
            _ => OrderStatus.Paid
        };
    }

    private static string BuildProductName(int index)
    {
        var brand = Brands[index % Brands.Length];
        var noun = ProductNouns[(index * 5) % ProductNouns.Length];
        var model = $"{(char)('A' + index % 26)}{100 + index}";
        return $"{brand} {noun} {model}";
    }

    private static decimal RoundMoney(decimal amount)
        => decimal.Round(amount, 2, MidpointRounding.AwayFromZero);

    private static Color GetSeedColor(int index)
    {
        var colorIndex = index % Colors.Length;
        return Color.Create(Colors[colorIndex], ColorHexCodes[colorIndex]);
    }

    private static DateTimeOffset MonthsAgo(int months)
        => DateTimeOffset.UtcNow.AddMonths(-months);

    private static string ToSlug(string value)
    {
        var chars = value
            .Trim()
            .ToLowerInvariant()
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
            .ToArray();

        var slug = new string(chars);
        while (slug.Contains("--", StringComparison.Ordinal))
            slug = slug.Replace("--", "-", StringComparison.Ordinal);

        return slug.Trim('-');
    }

    private static void Stamp(BaseEntity entity, DateTimeOffset createdAt)
        => entity.CreatedAt = createdAt;

    private static void SetPrivateProperty<T>(object instance, string propertyName, T value)
    {
        var property = instance.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        property?.SetValue(instance, value);
    }

    private static class StatusCodes
    {
        public const int Ok = 200;
    }
}
