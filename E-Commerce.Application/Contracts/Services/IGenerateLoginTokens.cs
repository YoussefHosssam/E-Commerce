using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Services
{
    public interface IGenerateLoginTokens
    {
        Task<(string accessToken, string refreshToken)> GenerateTokensAndSaveAsync(User user , CancellationToken ctn);
        Task<(string accessToken, RefreshToken refreshToken , string rawRefreshToken)> GenerateTokensAsync(User user, CancellationToken ctn);

    }
}
