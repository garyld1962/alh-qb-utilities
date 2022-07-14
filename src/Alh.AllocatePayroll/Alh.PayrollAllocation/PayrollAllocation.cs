using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alh.AllocatePayroll
{
    public class PayrollAllocation
    {

        public PayrollAllocation()
        {

        }

        public string Name { get; set; }
        public int Month { get; set; }
        public double BocaRatonAllocation { get; set; }
        public double BoyntonBeachAllocation { get; set; }
        public double CoralSpringsAllocation { get; set; }

    }

    public class PayrollDetails
    {
        public string Name { get; set; }
        public DateOnly PayDate { get; set; }
        public string Amount { get; set; }
       
    }

    public class PayrollEntry
    {
        public string InvoiceNumber { get; set; }
        public string Class { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
    }
}
