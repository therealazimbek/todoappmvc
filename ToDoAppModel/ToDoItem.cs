using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ToDoAppModel
{
    public class ToDoItem
    {
        public int Id { get; set; }
        [Required]
        [StringLength(60)]
        public string Title { get; set; }
        [StringLength(120)]
        public string Description { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:ddd, dd/MM/yyyy}")]
        public DateTime Created { get; set; }
        [CheckDateRange]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:ddd, dd/MM/yyyy}")]
        public DateTime DueDate { get; set; }
        public ItemStatus Status { get; set; }
        public int TodoListId { get; set; }
        [JsonIgnore]
        public virtual ToDoList ToDoList { get; set; }
    }

    public enum ItemStatus
    {
        Completed, InProgress, NotStarted
    }
}
