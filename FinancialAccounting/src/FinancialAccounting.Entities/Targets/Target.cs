namespace FinancialAccounting.Entities.Targets
{
    public class Target
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public decimal Total { get; set; } = 0;
        public required decimal Goal { get; set; }
    }
}
