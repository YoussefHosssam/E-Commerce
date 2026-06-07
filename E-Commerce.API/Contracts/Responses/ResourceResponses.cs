using E_Commerce.Application.Features.Auth.Commands.LoginUser;
using E_Commerce.Application.Features.Auth.Commands.RefreshUserToken;
using E_Commerce.Application.Features.Auth.Commands.TwoFactorAuth;
using E_Commerce.Application.Features.Cart.Common;
using E_Commerce.Application.Features.Category.Common;
using E_Commerce.Application.Features.ImageUploads.Common;
using E_Commerce.Application.Features.Me.Common;
using E_Commerce.Application.Features.Order.Common;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Application.Features.Variant.Common;

namespace E_Commerce.API.Contracts.Responses;

public sealed record AuthResponse(LoginUserResponse Auth);
public sealed record FinalizeAuthResponse(FinalizeLoginResponse Auth);
public sealed record RefreshAuthResponse(RefreshTokensResponse Auth);
public sealed record TwoFactorSetupResponse(SetupTwoFactorAuthResponse TwoFactorSetup);

public sealed record CartResponse(CartSummaryDTO Cart);

public sealed record CategoryResponse(CategoryDetailDto Category);
public sealed record CategoriesResponse(IReadOnlyCollection<CategoryListItemDto> Categories);

public sealed record ProductResponse(ProductDetailDto Product);
public sealed record ProductsResponse(IReadOnlyCollection<ProductListItemDto> Products);

public sealed record VariantResponse(VariantDetailDto Variant);
public sealed record VariantsResponse(IReadOnlyCollection<VariantListItemDto> Variants);
public sealed record VariantDetailsResponse(IReadOnlyCollection<VariantDetailDto> Variants);

public sealed record CheckoutResponse(CheckoutSummaryDto Checkout);
public sealed record CheckoutReviewResponse(CheckoutReviewDto Checkout);
public sealed record OrderPlacementResponse(PlaceOrderResponse Order);

public sealed record OrderResponse(OrderDto Order);
public sealed record OrdersResponse(IReadOnlyCollection<OrderListDto> Orders);
public sealed record PaymentResponse(PaymentDto Payment);

public sealed record UploadSignatureResponse(GenerateImageUploadSignatureResponse UploadSignature);
public sealed record ImageResponse(ImageDto Image);

public sealed record UserResponse(CurrentUserDto User);
public sealed record UserProfileResponse(UserProfileDto UserProfile);
public sealed record AddressResponse(UserAddressDto Address);
public sealed record AddressesResponse(IReadOnlyCollection<UserAddressDto> Addresses);
