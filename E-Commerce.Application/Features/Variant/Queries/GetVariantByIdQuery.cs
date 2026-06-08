using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;

namespace E_Commerce.Application.Features.Variant.Queries;

public sealed record GetVariantByIdQuery(Guid ProductId, Guid VariantId) : IRequest<Result<VariantDetailDto>>;

public sealed class GetVariantByIdValidation : AbstractValidator<GetVariantByIdQuery>
{
    public GetVariantByIdValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(VariantErrors.ProductRequired);
        RuleFor(x => x.VariantId).NotEmpty().WithError(VariantErrors.VariantIdRequired);
    }
}

public sealed class GetVariantByIdHandler : IRequestHandler<GetVariantByIdQuery, Result<VariantDetailDto>>
{
    private readonly IUnitOfWork _uow;

    public GetVariantByIdHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<VariantDetailDto>> Handle(GetVariantByIdQuery request, CancellationToken cancellationToken)
    {
        var variant = await _uow.Variants.GetVariantDetailsDtoAsync(request.ProductId, request.VariantId, cancellationToken);
        if (variant is null)
        {
            return Result<VariantDetailDto>.Fail(VariantErrors.NotFound);
        }

        return Result<VariantDetailDto>.Success(variant);
    }
}
