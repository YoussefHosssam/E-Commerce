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
        public Task<IReadOnlyCollection<Tobject>?> GetAllAsync(CancellationToken ctn);
        public Task<Tobject?> GetByIdAsync(Guid id, CancellationToken ctn);
        public Task<Tobject> CreateAsync(Tobject Object, CancellationToken ctn);
        public Task DeleteAsync(Guid id, CancellationToken ctn);
        public Task<Tobject?> GetSingleByPredicateAsync(Expression<Func<Tobject, bool>> predicate, CancellationToken ctn);
    }
}
