using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoAppFinal.Models;
using ToDoAppModel;

namespace ToDoAppFinal.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ToDoAppContext _context;

        public ItemsController(ToDoAppContext context)
        {
            _context = context;
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFoundPage", "Error");
            }

            var toDoItem = await _context.ToDoItems
                .Include(t => t.ToDoList)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoItem == null)
            {
                return RedirectToAction("NotFoundPage", "Error");
            }

            return View(toDoItem);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["TodoListId"] = new SelectList(_context.ToDoLists, "Id", "Name");
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,DueDate,TodoListId,Remind")] ToDoItem toDoItem)
        {
            if (ModelState.IsValid)
            {
                toDoItem.Created = DateTime.Now;
                toDoItem.Status = ItemStatus.NotStarted;
                toDoItem.IsHidden = false;
                var list_id = toDoItem.TodoListId;
                _context.Add(toDoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Home", new { id = list_id });
            }
            ViewData["TodoListId"] = new SelectList(_context.ToDoLists, "Id", "Name", toDoItem.TodoListId);
            return View(toDoItem);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFoundPage", "Error");
            }

            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem == null)
            {
                return RedirectToAction("NotFoundPage", "Error");
            }
            ViewData["TodoListId"] = new SelectList(_context.ToDoLists, "Id", "Name", toDoItem.TodoListId);
            return View(toDoItem);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,DueDate,Status,TodoListId,IsHidden,Remind")] ToDoItem toDoItem)
        {
            if (id != toDoItem.Id)
            {
                return RedirectToAction("NotFoundPage", "Error");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoItemExists(toDoItem.Id))
                    {
                        return RedirectToAction("NotFoundPage", "Error");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Home", new { id = toDoItem.TodoListId });
            }
            ViewData["TodoListId"] = new SelectList(_context.ToDoLists, "Id", "Name", toDoItem.TodoListId);
            return View(toDoItem);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFoundPage", "Error");
            }

            var toDoItem = await _context.ToDoItems
                .Include(t => t.ToDoList)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoItem == null)
            {
                return RedirectToAction("NotFoundPage", "Error");
            }

            return View(toDoItem);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            var list_id = toDoItem.TodoListId;
            _context.ToDoItems.Remove(toDoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Home", new { id = list_id });
        }

        public async Task<IActionResult> Complete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFoundPage", "Error");
            }

            var toDoItem = await _context.ToDoItems
                .FindAsync(id);
            if (toDoItem == null)
            {
                return RedirectToAction("NotFoundPage", "Error");
            }

            toDoItem.Status = ItemStatus.Completed;
            toDoItem.Remind = false;
            var list_id = toDoItem.TodoListId;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Home", new { id = list_id });
        }

        private bool ToDoItemExists(int id)
        {
            return _context.ToDoItems.Any(e => e.Id == id);
        }
    }
}
