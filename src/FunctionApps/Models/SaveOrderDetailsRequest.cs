using System;
using System.Collections.Generic;

namespace FunctionApps.Models
{
    public class SaveOrderDetailsRequest
    {
        public Guid Id { get; set; }
        public Dictionary<string, int> OrderItems { get; set; }
        public decimal TotalPay { get; set; }
        public string Adress { get; set; }
    }
}
