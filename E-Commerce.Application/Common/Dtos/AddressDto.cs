using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common.Dtos
{
    public sealed record ShippingAddressDto(
        string FullName,
        string Phone,
        string City,
        string AddressLine1,
        string? AddressLine2);
    public sealed record BillingAddressDto(
    string FullName,
    string Phone,
    string City,
    string AddressLine1,
    string? AddressLine2);
}
