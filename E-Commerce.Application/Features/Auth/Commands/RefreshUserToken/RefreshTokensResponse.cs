using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Auth.Commands.RefreshUserToken
{
    public record RefreshTokensResponse (string accessToken , string refreshToken)
    {
    }
}
