namespace FinancialAccouting.Contracts.Targets
{
    public record CreateTargetDto (Guid userId, string name, decimal total, decimal goal) {}
}
