using System.Collections.Generic;
using ToDoAppFinal.Models.ViewModels;
using ToDoAppModel;

namespace ToDoAppFinal.Models
{
    public class ViewModel
    {
        public ToDoList ToDoList { get; set; }
        public ToDoItem ToDoItem { get; set; }
        public IEnumerable<ToDoItem> ToDoItems { get; set; }
        public IEnumerable<ToDoList> ToDoLists { get; set; }
        public bool ShowHidden { get; set; } = false;
        public bool ShowCompletedTasks { get; set; } = false;
        public PagingInfo PagingInfo { get; set; }
        public int TodayListId { get; set; }
    }
}
