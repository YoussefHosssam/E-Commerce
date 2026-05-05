using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Common;
using E_Commerce.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_Commerce.Persistence.Repositories.Shared
{
    public class GenericRepository<Tobject> : IGenericRepository<Tobject>
        where Tobject : BaseEntity
    {
        private readonly EcommerceContext _context;
        private readonly DbSet<Tobject> _set;

        public GenericRepository(EcommerceContext context)
        {
            _context = context;
            _set = _context.Set<Tobject>();
        }

        public async Task<IReadOnlyCollection<Tobject>?> GetAllAsync(CancellationToken ctn)
        {
            // Read-only query: no tracking for performance
            return await _set
                .AsNoTracking()
                .ToListAsync(ctn);
        }

        public async Task<Tobject?> GetByIdAsync(Guid id, CancellationToken ctn)
        {
            var entity = await ReturnIfExist(id, ctn);
            return entity;
        }

        public async Task<Tobject> CreateAsync(Tobject Object, CancellationToken ctn)
        {
            await _set.AddAsync(Object, ctn);
            return Object;
        }
        public async Task DeleteAsync(Guid id, CancellationToken ctn)
        {
            var entity = await ReturnIfExist(id, ctn);
            _set.Remove(entity);
        }
        private async Task<Tobject?> ReturnIfExist(Guid id , CancellationToken ctn)
        {
            var entity = await _set.FirstOrDefaultAsync(o => o.Id == id, ctn);
            return entity;
        }
        public async Task<Tobject?> GetSingleByPredicateAsync(Expression<Func<Tobject, bool>> predicate, CancellationToken ctn)
        {
            var entity = await _set.FirstOrDefaultAsync(predicate, ctn);
            return entity;
        }

        public async Task CreateRangeAsync(IEnumerable<Tobject> entities, CancellationToken ct)
        {
            await _set.AddRangeAsync(entities, ct);
        }
    }
}