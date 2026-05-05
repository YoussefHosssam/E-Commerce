using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CartEntity = E_Commerce.Domain.Entities.Cart;
using System.Threading.Tasks;
using E_Commerce.Domain.Common.Errors;
using AutoMapper;

namespace E_Commerce.Application.Features.Checkout.Queries
{
    public class GetCheckoutSummaryHandler : IRequestHandler<GetCheckoutSummaryQuery, Result<CheckoutSummaryDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;

        public GetCheckoutSummaryHandler(IUnitOfWork uow, IUserAccessor userAccessor, IMapper mapper)
        {
            _uow = uow;
            _userAccessor = userAccessor;
            _mapper = mapper;
        }

        public async Task<Result<CheckoutSummaryDto>> Handle(GetCheckoutSummaryQuery request, CancellationToken cancellationToken)
        {
            CartEntity? cart = await GetUserCart(cancellationToken);
            if (cart is null || !cart.Items.Any())
                return Result<CheckoutSummaryDto>.Fail(CheckoutErrors.EmptyCart);
            CheckoutSummaryDto checkoutSummary = _mapper.Map<CheckoutSummaryDto>(cart);
            return Result<CheckoutSummaryDto>.Success(checkoutSummary);
        }
        private async Task<CartEntity?> GetUserCart(CancellationToken ct)
        {
            Guid userId = _userAccessor.UserId!.Value;
            CartEntity? cart = await _uow.Carts.GetCartWithItemsByUserId(userId, ct);
            return cart;
        }
    }
}
