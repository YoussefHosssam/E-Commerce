using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Order.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;
using FluentValidation;
using MediatR;

namespace E_Commerce.Application.Features.Order.Queries;

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

    public GetOrderByIdHandler(IUserAccessor userAccessor, IUnitOfWork uow)
    {
        _userAccessor = userAccessor;
        _uow = uow;
    }

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetRequiredUserId();
        var userRole = _userAccessor.GetRequiredRole();
        var order = await _uow.Orders.GetOrderDetailsDtoAsync(request.id, cancellationToken);
        if (order is null)
            return Result<OrderDto>.Fail(OrderErrors.NotFound);

        if (userRole != UserRole.Admin && userId != order.UserId)
            return Result<OrderDto>.Fail(OrderErrors.NotFound);

        return Result<OrderDto>.Success(order.Order);
    }
}
