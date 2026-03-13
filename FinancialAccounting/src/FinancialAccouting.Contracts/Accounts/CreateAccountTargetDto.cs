using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccouting.Contracts
{
    public record CreateAccountTargetDto(string Name, decimal Total, decimal Goal);
}
