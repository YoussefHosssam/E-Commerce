using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity
{
    public interface IUserAccessor
    {
        Guid? UserId { get; }
        EmailAddress? Email { get; }
        UserRole? Role { get; }
        bool IsAuthenticated { get; }
    }
}
