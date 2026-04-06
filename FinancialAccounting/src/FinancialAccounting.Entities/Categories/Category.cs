namespace FinancialAccounting.Entities.Categories
{
    public class Category
    {
        public Category(Guid id, string name, CategoryTypes type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public CategoryTypes Type { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
