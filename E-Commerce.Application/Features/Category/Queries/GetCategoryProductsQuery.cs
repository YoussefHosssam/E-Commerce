using AutoMapper;
using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Category.Queries
{
    public record GetCategoryProductsQuery(Guid categoryId , PageRequest page) : IRequest<Result<IReadOnlyCollection<ProductListItemDto>>>;
    public class GetCategoryProductsValidation : AbstractValidator<GetCategoryProductsQuery>
    {
        public GetCategoryProductsValidation()
        {
            RuleFor(q => q.categoryId)
                .NotNull()
                .WithError(CategoryErrors.IdRequired);
        }
    }

    public class GetCategoryProductsHandler : IRequestHandler<GetCategoryProductsQuery, Result<IReadOnlyCollection<ProductListItemDto>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetCategoryProductsHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyCollection<ProductListItemDto>>> Handle(GetCategoryProductsQuery request, CancellationToken cancellationToken)
        {
            var categoryProducts = await _uow.Categories.GetProductsForCategory(request.categoryId, request.page, cancellationToken);
            if (categoryProducts is null || categoryProducts.Items is null)
                return Result<IReadOnlyCollection<ProductListItemDto>>.Fail(ProductErrors.NotFound);
            IReadOnlyCollection <ProductListItemDto> products = _mapper.Map<IReadOnlyCollection<ProductListItemDto>>(categoryProducts.Items);
            return Result<IReadOnlyCollection<ProductListItemDto>>.Success(products , categoryProducts.ToMetaResult());
        }
    }
}
