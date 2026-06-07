using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Persistence.Shared
{
    public interface ITransactionManager
    {
        Task ExecuteAsync(
            Func<CancellationToken, Task> operation,
            CancellationToken ct = default);

        Task<T> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken ct = default);
    }
}
