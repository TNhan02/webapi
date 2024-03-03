using System;
using System.Collections.Generic;

namespace webapi.Models
{
    public partial class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
