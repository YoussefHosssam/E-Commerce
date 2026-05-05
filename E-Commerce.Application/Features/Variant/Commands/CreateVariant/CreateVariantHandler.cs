using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using AutoMapper;

namespace E_Commerce.Application.Features.Variant.Commands.CreateVariant;

public sealed class CreateVariantHandler : IRequestHandler<CreateVariantCommand, Result<VariantDetailDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public CreateVariantHandler(IUnitOfWork uow, IUserAccessor userAccessor, IMapper mapper)
    {
        _uow = uow;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<Result<VariantDetailDto>> Handle(CreateVariantCommand request, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, true, cancellationToken);
        if (product is null)
        {
            return Result<VariantDetailDto>.Fail(ProductErrors.NotFound);
        }

        if (await _uow.Variants.SkuExistsAsync(request.Sku, null, cancellationToken))
        {
            return Result<VariantDetailDto>.Fail(VariantErrors.SkuDuplicate);
        }

        Money? priceOverride = null;
        if (request.PriceOverrideAmount.HasValue)
        {
            priceOverride = Money.Create(request.PriceOverrideAmount.Value, CurrencyCode.Create(request.PriceOverrideCurrency!));
        }

        var variant = product.AddVariant(request.Sku, request.Size, request.Color, priceOverride, now);
        if (!request.IsActive)
        {
            variant.Deactivate();
        }
        await CreateInventoryForNewVariant(variant.Id, request.stock, cancellationToken , now);
        await CreateStockMovementForNewVariant(variant.Id, request.stock, cancellationToken , now);
        await _uow.SaveChangesAsync(cancellationToken);
        var createdVariant = await _uow.Variants.GetByIdWithDetailsAsync(variant.Id, cancellationToken);
        return Result<VariantDetailDto>.Success(_mapper.Map<VariantDetailDto>(createdVariant ?? variant));
    }

    private async Task CreateStockMovementForNewVariant(Guid variantId, int stock, CancellationToken cancellationToken , DateTimeOffset now)
    {
        StockMovement stockMovement = StockMovement.Create(variantId, StockMovementType.InitialStock, stock, "New Variant has beed added.", variantId,  _userAccessor.UserId , now);
        await _uow.StockMovements.CreateAsync(stockMovement, cancellationToken);
    }

    private async Task CreateInventoryForNewVariant(Guid variantId, int initStock , CancellationToken cancellationToken, DateTimeOffset now)
    {
        Inventory inventory = Inventory.Create(variantId, initStock, now);
        await _uow.Inventories.CreateAsync(inventory, cancellationToken);
    }
}

