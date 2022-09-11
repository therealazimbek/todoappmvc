using NUnit.Framework;
using System;
using ToDoAppModel;

namespace ToDoDomainTests
{
    /// <summary>
    /// Test class
    /// </summary>
    [TestFixture]
    public class Tests
    {
        private ToDoList toDoList;
        private ToDoItem toDoItem;

        [SetUp]
        public void Setup()
        {
            toDoList = new ToDoList
            {
                Id = 1,
                Name = "Personal"
            };
            toDoItem = new ToDoItem
            {
                Title = "hw",
                Description = "college hw",
                Created = System.DateTime.Now,
                DueDate = new System.DateTime(2022, 9, 14),
                Status = ItemStatus.NotStarted,
                TodoListId = 1
            };
        }

        /// <summary>
        /// Ensure basic props
        /// </summary>
        [Test]
        public void EnsureInitial()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.toDoList.Name, Is.EqualTo("Personal"));
                Assert.That(this.toDoItem.Status, Is.EqualTo(ItemStatus.NotStarted));
            });
        }

        /// <summary>
        /// list creation success
        /// </summary>
        [Test]
        public void CreateListSuccess()
        {
            Assert.DoesNotThrow(() => new ToDoList { Id = 10, Name = "Test" });
        }

        /// <summary>
        /// item creation success
        /// </summary>
        [Test]
        public void CreateItemSuccess()
        {
            Assert.DoesNotThrow(() => new ToDoItem
            {
                Id = 10,
                Title = "hwd",
                Description = "college hwd",
                Created = System.DateTime.Now,
                DueDate = new System.DateTime(2022, 9, 24),
                Status = ItemStatus.NotStarted,
                TodoListId = 1
            });
        }
    }
}