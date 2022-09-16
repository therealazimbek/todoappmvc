using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ToDoAppFinal.Models;
using ToDoAppModel;

namespace ToDoAppFinal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ToDoAppContext _context;
        public const int PageSize = 4;

        public HomeController(ILogger<HomeController> logger, ToDoAppContext context)
        {
            _logger = logger;
            this._context = context;
        }

        public async Task<IActionResult> Index(int listPage = 1)
        {
            var view = new ViewModel
            {
                ToDoItems = await _context.ToDoItems.ToListAsync(),
                ShowHidden = false,
                ToDoLists = await _context.ToDoLists.Where(l => l.IsHidden == false).Take(4).ToListAsync(),
                TodayListId = _context.ToDoLists.FirstOrDefault(l => l.Name == "Today").Id
            };

            return View(view);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
