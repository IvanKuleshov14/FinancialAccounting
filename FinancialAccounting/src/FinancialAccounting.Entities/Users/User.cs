namespace FinancialAccounting.Entities.Users
{
    public class User
    {
        public User(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
