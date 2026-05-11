using AutoMapper;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Order.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Order.Queries
{
    public record GetOrderByIdQuery(Guid id) : IRequest<Result<OrderDto>>;
    public class GetOrderByIdValidation : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdValidation()
        {
            RuleFor(r => r.id)
                .NotEmpty()
                .WithError(OrderErrors.IdRequired);
        }
    }
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetOrderByIdHandler(IUserAccessor userAccessor, IUnitOfWork uow , IMapper mapper)
        {
            _userAccessor = userAccessor;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _userAccessor.UserId;
            var userRole = _userAccessor.Role;
            if (!userId.HasValue) return Result<OrderDto>.Fail(AuthErrors.InvalidToken);
            if (!userRole.HasValue) return Result<OrderDto>.Fail(AuthErrors.InvalidToken);
            var order = await _uow.Orders.GetOrderByIdWithDetailsAsync(request.id, cancellationToken);
            if (order is null) return Result<OrderDto>.Fail(OrderErrors.NotFound);
            OrderDto orderDto = _mapper.Map<OrderDto>(order);
            if (userRole.Value == UserRole.Admin) return Result<OrderDto>.Success(orderDto);
            else
            {
                if (userId != order.UserId) return Result<OrderDto>.Fail(OrderErrors.NotFound);
            }
            return Result<OrderDto>.Success(orderDto);
        }
    }
}
