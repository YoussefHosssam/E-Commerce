using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastrucuture.Cart
{
    public interface ICartMergeService
    {
        (bool , string) IsNeedToBeMerged();
    }
}
