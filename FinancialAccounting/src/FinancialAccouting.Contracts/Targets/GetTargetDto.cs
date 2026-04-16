namespace FinancialAccouting.Contracts.Targets
{
    public record GetTargetDto (Guid Id, string Name, decimal Total, decimal Goal, decimal Progress) {}
}