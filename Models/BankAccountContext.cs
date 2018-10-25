using Microsoft.EntityFrameworkCore;

namespace Bank_Accounts.Models
{
    public class BankAccountContext : DbContext
    {
        public BankAccountContext(DbContextOptions<BankAccountContext> options) : base(options) { }
        public DbSet<User> users {get;set;}
        public DbSet<Transaction> transactions {get;set;}
    }
}