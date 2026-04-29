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
        RuleFor(x => x.ProductId).NotEmpty().WithError(ErrorCodes.Variant.ProductRequired);
        RuleFor(x => x.VariantId).NotEmpty().WithError(ErrorCodes.Variant.VariantIdRequired);
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
        var variant = await _uow.Variants.GetByIdWithDetailsAsync(request.VariantId, cancellationToken);
        if (variant is null || variant.ProductId != request.ProductId)
        {
            return Result<VariantDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Variant.NotFound));
        }

        return Result<VariantDetailDto>.Success(variant.ToDetailDto());
    }
}
