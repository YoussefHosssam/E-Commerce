using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        protected BaseEntity()
        {
            this.Id = Guid.NewGuid();
            this.CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}
