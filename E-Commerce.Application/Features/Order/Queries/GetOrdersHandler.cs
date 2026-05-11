using AutoMapper;
using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Features.Order.Common;
using E_Commerce.Application.Features.Product.Queries.GetProducts;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Order.Queries
{

    public record GetOrdersQuery(PageRequest page) : IRequest<Result<List<OrderListDto>>>;

    public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, Result<List<OrderListDto>>>
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetOrdersHandler(IUserAccessor userAccessor, IUnitOfWork uow, IMapper mapper)
        {
            _userAccessor = userAccessor;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<List<OrderListDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var userId = _userAccessor.UserId;
            if (!userId.HasValue) return Result<List<OrderListDto>>.Fail(AuthErrors.InvalidToken);
            var pagedOrders = await _uow.Orders.GetOrdersWithDetailsAsync(userId.Value, request.page, cancellationToken);
            List<OrderListDto> orders = _mapper.Map<List<OrderListDto>>(pagedOrders.Items);
            return Result<List<OrderListDto>>.Success(orders, pagedOrders.ToMetaResult());
        }
    }
}
