namespace FinancialAccouting.Contracts.Categories
{
    public record GetCategoryListDto (
        Guid Id,
        string Name,
        int Type
        ) {}
}
