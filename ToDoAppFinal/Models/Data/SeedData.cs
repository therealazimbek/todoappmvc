using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ToDoAppFinal.Models.Data
{
    public class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            ToDoAppContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<ToDoAppContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            if (!context.ToDoLists.Any())
            {
                context.ToDoLists.AddRange(
                    new ToDoAppModel.ToDoList
                    {
                        Name = "Personal",
                        IsHidden = false
                    },
                    new ToDoAppModel.ToDoList
                    {
                        Name = "Today",
                        IsHidden = false
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
