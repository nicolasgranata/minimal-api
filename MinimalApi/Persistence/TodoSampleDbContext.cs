using Microsoft.EntityFrameworkCore;
using MinimalApi.Models;

namespace MinimalApi.Persistence
{
    internal class TodoSampleDbContext : DbContext
    {
        public TodoSampleDbContext(DbContextOptions<TodoSampleDbContext> options)
            : base(options)
        {

        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
