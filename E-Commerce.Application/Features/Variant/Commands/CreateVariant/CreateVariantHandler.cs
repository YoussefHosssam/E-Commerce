using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Common.Errors;
using MediatR;
using AutoMapper;

namespace E_Commerce.Application.Features.Variant.Commands.CreateVariant;

public sealed class CreateVariantHandler : IRequestHandler<CreateVariantCommand, Result<IReadOnlyCollection<VariantDetailDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IVariantService _variantService;

    public CreateVariantHandler(IUnitOfWork uow, IMapper mapper, IVariantService variantService)
    {
        _uow = uow;
        _mapper = mapper;
        _variantService = variantService;
    }

    public async Task<Result<IReadOnlyCollection<VariantDetailDto>>> Handle(CreateVariantCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetAggregateByIdAsync(request.ProductId, true, cancellationToken);
        if (product is null)
        {
            return Result<IReadOnlyCollection<VariantDetailDto>>.Fail(ProductErrors.NotFound);
        }

        if (!product.HasVariants)
        {
            return Result<IReadOnlyCollection<VariantDetailDto>>.Fail(ProductErrors.CannotAddVariantsToSimpleProduct);
        }

        var result = await _variantService.CreateVariantsForProductAsync(product, request.Variants, cancellationToken);
        if (!result.IsSuccess)
            return Result<IReadOnlyCollection<VariantDetailDto>>.Fail(result.Error!);

        await _uow.SaveChangesAsync(cancellationToken);
        return Result<IReadOnlyCollection<VariantDetailDto>>.Success(_mapper.Map<IReadOnlyCollection<VariantDetailDto>>(result.Data!));
    }
}

