using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ToDoAppFinal.Models;
using ToDoAppModel;

namespace ToDoAppFinal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ToDoAppContext _context;
        public const int PageSize = 3;
        public const int ItemPageSize = 5;

        public HomeController(ToDoAppContext context)
        {
            _context = context;
        }

        // GET: Home
        public async Task<IActionResult> Index(bool? showHidden, int page = 1)
        {
            var view = new ViewModel
            {
                ToDoItems = await _context.ToDoItems.ToListAsync()
            };

            view.TodayListId = _context.ToDoLists.FirstOrDefault(l => l.Name == "Today").Id;

            if (showHidden.HasValue)
            {
                view.PagingInfo = new Models.ViewModels.PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = _context.ToDoLists.Count(l => l.IsHidden == true)
                };

                view.ToDoLists = await _context.ToDoLists
                        .Where(l => l.IsHidden == true).Skip((page - 1) * PageSize)
                        .Take(PageSize).ToListAsync();
                view.ShowHidden = true;

                return View(view);
            }

            view.ShowHidden = false;
            view.ToDoLists = await _context.ToDoLists.Where(l => l.IsHidden == false && l.Name != "Today").Skip((page - 1) * PageSize)
                        .Take(PageSize).ToListAsync();
            view.PagingInfo = new Models.ViewModels.PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = PageSize,
                TotalItems = _context.ToDoLists.Count(l => l.IsHidden == false) - 1
            };
            return View(view);
        }

        // GET: Home/Details/5
        public async Task<IActionResult> Details(int? id, bool? showCompletedTasks, int page = 1)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoList == null)
            {
                return NotFound();
            }

            var view = new ViewModel
            {
                ToDoList = toDoList,
            };

            if (showCompletedTasks.HasValue.Equals(true))
            {
                view.PagingInfo = new Models.ViewModels.PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = ItemPageSize,
                    TotalItems = _context.ToDoItems.Count(l => l.TodoListId == id && l.Status == ItemStatus.Completed)
                };
                if (toDoList.Name == "Today")
                {
                    view.ToDoItems = await _context.ToDoItems.Where(i => i.DueDate.Date == System.DateTime.Today && i.Status == ItemStatus.Completed).OrderByDescending(i => i.DueDate).Skip((page - 1) * ItemPageSize)
                        .Take(ItemPageSize).ToListAsync();
                }
                else
                {
                    view.ToDoItems = await _context.ToDoItems.Where(i => i.TodoListId == id && i.Status == ItemStatus.Completed).OrderByDescending(i => i.DueDate).Skip((page - 1) * ItemPageSize)
                        .Take(ItemPageSize).ToListAsync();
                }
                view.ShowCompletedTasks = true;
                return View(view);
            }

            view.ShowCompletedTasks = false;
            view.ToDoItems = await _context.ToDoItems.Where(i => i.Status != ItemStatus.Completed).ToListAsync();
            if (toDoList.Name == "Today")
            {
                view.ToDoItems = view.ToDoItems.Where(i => i.DueDate.Date == System.DateTime.Today).Skip((page - 1) * ItemPageSize)
                        .Take(ItemPageSize).ToList();
            }
            else
            {
                view.ToDoItems = view.ToDoItems.Where(i => i.TodoListId == id).Skip((page - 1) * ItemPageSize)
                        .Take(ItemPageSize).ToList();
            }

            view.PagingInfo = new Models.ViewModels.PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = ItemPageSize,
                TotalItems = _context.ToDoItems.Count(i => i.TodoListId == id && i.Status != ItemStatus.Completed)
            };

            return View(view);
        }

        // GET: Home/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ToDoList toDoList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(toDoList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(toDoList);
        }

        // GET: Home/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoLists.FindAsync(id);
            if (toDoList == null)
            {
                return NotFound();
            }
            return View(toDoList);
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsHidden")] ToDoList toDoList)
        {
            if (id != toDoList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDoList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoListExists(toDoList.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(toDoList);
        }

        // GET: Home/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoLists
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoList == null)
            {
                return NotFound();
            }

            return View(toDoList);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toDoList = await _context.ToDoLists.FindAsync(id);
            _context.ToDoLists.Remove(toDoList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Hide(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoList = await _context.ToDoLists
                .FindAsync(id);
            if (toDoList == null)
            {
                return NotFound();
            }

            toDoList.IsHidden = !toDoList.IsHidden;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        private bool ToDoListExists(int id)
        {
            return _context.ToDoLists.Any(e => e.Id == id);
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
