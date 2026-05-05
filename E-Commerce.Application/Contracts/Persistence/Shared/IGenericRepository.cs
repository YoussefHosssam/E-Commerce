using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Persistence.Shared
{
    public interface IGenericRepository<Tobject> where Tobject : BaseEntity 
    {
        Task<IReadOnlyCollection<Tobject>?> GetAllAsync(CancellationToken ctn);
        Task<Tobject?> GetByIdAsync(Guid id, CancellationToken ctn);
        Task<Tobject> CreateAsync(Tobject Object, CancellationToken ctn);
        Task DeleteAsync(Guid id, CancellationToken ctn);
        Task<Tobject?> GetSingleByPredicateAsync(Expression<Func<Tobject, bool>> predicate, CancellationToken ctn);
        Task CreateRangeAsync(IEnumerable<Tobject> entities, CancellationToken ct);

    }
}
