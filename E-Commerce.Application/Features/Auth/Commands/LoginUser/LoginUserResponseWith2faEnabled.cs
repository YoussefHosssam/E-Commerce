using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Auth.Commands.LoginUser
{
    public record LoginUserResponseWith2faEnabled  : LoginUserResponse
    {
        public LoginUserResponseWith2faEnabled(string twoFactorAuthChallengeId, bool requiresTwoFactorAuth = true) : base(new {twoFactorAuthChallengeId , requiresTwoFactorAuth})
        {
            
        }
    }
}
