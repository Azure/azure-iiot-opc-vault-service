namespace todo.Models
{
    using Microsoft.EntityFrameworkCore;

    public class Context : DbContext
    {
        public Context() : base()
        {
        }

        public DbSet<Item> Items { get; set; }
    }
}
