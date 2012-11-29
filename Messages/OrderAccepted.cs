using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class OrderAccepted
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public double OrderValue { get; set; }

        public int OrderId { get; set; }
    }
}
