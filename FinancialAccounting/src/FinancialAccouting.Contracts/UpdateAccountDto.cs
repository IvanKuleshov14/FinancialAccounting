using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccouting.Contracts
{
    public record UpdateAccountDto(string Name, Guid UserId, decimal Total);
}
