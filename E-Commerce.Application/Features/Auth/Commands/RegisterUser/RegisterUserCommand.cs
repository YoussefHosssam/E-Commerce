using E_Commerce.Application.Common.Result;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Auth.Commands.RegisterUser
{
    public record RegisterUserCommand (string FirstName , string LastName , string Email , string Password , string? PhoneNumber ) : IRequest<Result>;
}
