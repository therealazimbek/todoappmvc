using Microsoft.EntityFrameworkCore;
using ToDoAppModel;

namespace ToDoAppBeta.Models
{
    public class ToDoAppContext : DbContext
    {
        public ToDoAppContext(DbContextOptions<ToDoAppContext> options)
            : base(options) { }

        public DbSet<ToDoList> ToDoLists { get; set; }
        public DbSet<ToDoItem> ToDoItems { get; set; }
    }
}
