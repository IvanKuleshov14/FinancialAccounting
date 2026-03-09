using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Entities.Categories
{
    public class Category
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required CategoryTypes Type { get; set; }
    }
}
