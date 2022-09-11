using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoAppModel
{
    public class ToDoList
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public virtual List<ToDoItem> Items { get; set; }
    }
}
