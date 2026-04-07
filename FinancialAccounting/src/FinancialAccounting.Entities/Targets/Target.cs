namespace FinancialAccounting.Entities.Targets
{
    public class Target
    {
        public Target(Guid id, Guid userId, string name, decimal total, decimal goal)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Total = total;
            Goal = goal;
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public decimal Total { get; set; } = 0;
        public decimal Goal { get; set; }
    }
}
