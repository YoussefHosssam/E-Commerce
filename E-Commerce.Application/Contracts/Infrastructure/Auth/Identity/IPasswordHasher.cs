using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity
{
    public interface IPasswordHasherAdapter
    {
        PasswordHash Hash(string password);
        bool Verify(PasswordHash hash, string password);
        
    }
}
