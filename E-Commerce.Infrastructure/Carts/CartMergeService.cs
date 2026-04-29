using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Carts
{
    internal class CartMergeService : ICartMergeService
    {
        private readonly ICartSessionService _session;
        public CartMergeService(ICartSessionService session)
        {
            _session = session;
        }

        public (bool, string) IsNeedToBeMerged()
        {
            var anonToken = _session.GetAnonymousId();
            if (string.IsNullOrEmpty(anonToken))
            {
                return (false, string.Empty);
            }
            return (true, anonToken);
        }
    }
}
