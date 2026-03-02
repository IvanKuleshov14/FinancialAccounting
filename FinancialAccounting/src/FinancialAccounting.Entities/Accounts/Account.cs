using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialAccounting.Entities.Account;

internal class Account
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public decimal Total { get; set; } = 0;

    public AccountTarget? Target { get; set; }
}
