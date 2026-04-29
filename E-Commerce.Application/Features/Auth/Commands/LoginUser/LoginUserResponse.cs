using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Auth.Commands.LoginUser
{
    public abstract record LoginUserResponse
    {
        public object Data { get; set; }
        protected LoginUserResponse(object data)
        {
            Data = data;
        }
    }
}
