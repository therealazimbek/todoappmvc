using System.Collections.Generic;
using ToDoAppModel;

namespace ToDoAppBeta.Models
{
    public class ViewModel
    {
        public ToDoList ToDoList { get; set; }
        public ToDoItem ToDoItem { get; set; }
        public IEnumerable<ToDoItem> ToDoItems { get; set; }
    }
}
