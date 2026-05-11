using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Category.Queries
{
    public record GetCategoryProductsQuery(Guid categoryId , PageRequest page) : IRequest<Result<List<ProductListItemDto>>>
    {
    }
    public class GetCategoryProductsHandler : IRequestHandler<GetCategoryProductsQuery, Result<List<ProductListItemDto>>>
    {
        public Task<Result<List<ProductListItemDto>>> Handle(GetCategoryProductsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
