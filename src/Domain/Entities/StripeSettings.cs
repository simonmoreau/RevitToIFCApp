using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class StripeSettings
    {
        public string ApiKey { get; set; }
        public Dictionary<string, StripeProduct> Products { get; set; }
    }

    public class StripeProduct
    {
        public int Quantity { get; set; }
        public string Name { get; set; }
    }
}
