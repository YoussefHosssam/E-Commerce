using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;
using AutoMapper;

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
    private readonly IMapper _mapper;

    public GetVariantByIdHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<VariantDetailDto>> Handle(GetVariantByIdQuery request, CancellationToken cancellationToken)
    {
        var variant = await _uow.Variants.GetByIdWithDetailsAsync(request.VariantId, cancellationToken);
        if (variant is null || variant.ProductId != request.ProductId)
        {
            return Result<VariantDetailDto>.Fail(VariantErrors.NotFound);
        }

        return Result<VariantDetailDto>.Success(_mapper.Map<VariantDetailDto>(variant));
    }
}
