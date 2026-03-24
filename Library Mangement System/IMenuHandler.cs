using Library_Management_System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal interface IMenuHandler
    {
        void Run();
        void ShowMainMenu();
    }

    internal class MenuHandler : IMenuHandler
    {
        private readonly ILibraryService _service;
        private readonly INotifier _notifier;
        public MenuHandler(ILibraryService service, INotifier notifier)
        {
            _service = service;
            _notifier = notifier;
        }
        public void Run()
        {
            bool running = true;
            while (running)
            {
                ShowMainMenu();
                var choice = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(choice))
                {
                    _notifier.Notify("Please enter a valid option.");
                    continue; // goes back to top of loop
                }
                switch (choice)
                {
                    case "1":
                        _notifier.Notify("Adding a new book...");
                        Console.Write("Title: ");
                        var title = Console.ReadLine();
                        Console.Write("Author: ");
                        var author = Console.ReadLine();
                        _service.AddBook(title, author);
                        break;
                    case "2":
                        _notifier.Notify("Registering a new member...");
                        Console.Write("Name: ");
                        var name = Console.ReadLine();
                        Console.Write("Membership Type: ");
                        var type = Console.ReadLine();
                        _service.RegisterMember(name, type);
                        break;
                    case "3":
                        _notifier.Notify("Borrowing a book...");
                        Console.Write("Book ID: ");
                        var bookId = Console.ReadLine();
                        Console.Write("Member ID: ");
                        var memberId = Console.ReadLine();
                        _service.BorrowBook(bookId, memberId);
                        break;
                    case "4":
                        _notifier.Notify("Returning a book...");
                        Console.Write("Book ID: ");
                        var returnBookId = Console.ReadLine();
                        _service.ReturnBook(returnBookId);
                        break;
                    case "5":
                        _notifier.Notify("Listing all books...");
                        var books = _service.GetAllBooks();
                        foreach (var book in books)
                            _notifier.Notify($"{book.ItemId}: {book.Title} by {book.Author} - {(book.IsAvailable() ? "Available" : "Borrowed")}");
                        break;
                    case "6":
                        _notifier.Notify("Listing all members...");
                        var members = _service.GetAllMembers();
                        foreach (var member in members)
                            _notifier.Notify($"{member.Id}: {member.Name} ({member.MembershipType})");
                        break;
                    case "7":
                        _notifier.Notify("Showing borrow history...");
                        var history = _service.GetBorrowHistory();
                        foreach (var record in history)
                            _notifier.Notify($"Book ID: {record.BookId}, Member ID: {record.MemberId}, Borrowed On: {record.BorrowedOn:dd MMM yyyy}, Due: {record.DueDate:dd MMM yyyy}");
                        break;
                    case "8":
                        _notifier.Notify("Searching...");
                        Console.Write("Enter title or author: ");
                        var query = Console.ReadLine();
                        var result = _service.SearchBook(query);
                        foreach (var book in result)
                        {
                            _notifier.Notify(book.Search());
                        }
                        break;
                    case "9":
                        _notifier.Notify("Exiting the system. Goodbye!");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
        public void ShowMainMenu()
        {
            Console.WriteLine("\nLibrary Management System");
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. Register Member");
            Console.WriteLine("3. Borrow a Book");
            Console.WriteLine("4. Return Book");
            Console.WriteLine("5. View All Books");
            Console.WriteLine("6. View All Members");
            Console.WriteLine("7. View Borrow History");
            Console.WriteLine("8. Search for a book by the title or author");
            Console.WriteLine("9. Exit");
        }
    }
}
