using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Auth.Commands.LoginUser
{
    public record FinalizeLoginResponse  : LoginUserResponse
    {
        public FinalizeLoginResponse(
            string accessToken, 
            string refreshToken, 
            DateTimeOffset LoggedInAt, 
            bool requiresTwoFactorAuth = false, 
            bool promptTwoFactorAuth = true) 
            : base(new {accessToken , refreshToken , LoggedInAt , requiresTwoFactorAuth , promptTwoFactorAuth})
        {
            
        }
    }
}
