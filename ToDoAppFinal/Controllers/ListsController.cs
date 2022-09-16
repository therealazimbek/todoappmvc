using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ToDoAppFinal.Models;
using ToDoAppModel;

namespace ToDoAppFinal.Controllers
{
    public class ListsController : Controller
    {
        private readonly ToDoAppContext _context;
        public const int PageSize = 4;

        public ListsController(ToDoAppContext context)
        {
            _context = context;
        }

        // GET: Lists
        public async Task<IActionResult> Index(bool? showHidden, int listPage = 1)
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
                    CurrentPage = listPage,
                    ItemsPerPage = _context.ToDoLists.Count(l => l.IsHidden == true),
                    TotalItems = _context.ToDoLists.Count(l => l.IsHidden == true)
                };

                view.ToDoLists = await _context.ToDoLists
                        .Where(l => l.IsHidden == true).ToListAsync();
                view.ShowHidden = true;

                return View(view);
            }

            view.ShowHidden = false;
            view.ToDoLists = await _context.ToDoLists.Where(l => l.IsHidden == false && l.Name != "Today").Skip((listPage - 1) * PageSize)
                        .Take(PageSize).ToListAsync();
            view.PagingInfo = new Models.ViewModels.PagingInfo
            {
                CurrentPage = listPage,
                ItemsPerPage = PageSize,
                TotalItems = _context.ToDoLists.Count(l => l.IsHidden == false)
            };
            return View(view);
        }

        // GET: Lists/Details/5
        public async Task<IActionResult> Details(int? id, bool? showCompletedTasks)
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

            if (showCompletedTasks.HasValue)
            {
                if (toDoList.Name == "Today")
                {
                    view.ToDoItems = await _context.ToDoItems.Where(i => i.DueDate.Date == System.DateTime.Today).ToListAsync();
                }
                else
                {
                    view.ToDoItems = await _context.ToDoItems.Where(i => i.TodoListId == id).ToListAsync();
                }
                view.ShowCompletedTasks = true;
                return View(view);
            }

            view.ShowCompletedTasks = false;
            view.ToDoItems = await _context.ToDoItems.Where(i => i.Status != ItemStatus.Completed).ToListAsync();
            if (toDoList.Name == "Today")
            {
                view.ToDoItems = view.ToDoItems.Where(i => i.DueDate.Date == System.DateTime.Today).ToList();
            }
            else
            {
                view.ToDoItems = view.ToDoItems.Where(i => i.TodoListId == id).ToList();
            }

            return View(view);
        }

        // GET: Lists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Lists/Edit/5
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

        // POST: Lists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Lists/Delete/5
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

        // POST: Lists/Delete/5
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

            return RedirectToAction("Index", "Lists");
        }

        private bool ToDoListExists(int id)
        {
            return _context.ToDoLists.Any(e => e.Id == id);
        }
    }
}
