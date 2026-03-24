using Library_Management_System;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Library_Management_System
{
    internal interface ILibraryService
    {
        void AddBook(string title, string author);
        void RegisterMember(string name, string membershipType);
        void BorrowBook(string bookId, string memberId);
        void ReturnBook(string bookId);
        List<Book> GetAllBooks();
        List<Member> GetAllMembers();
        List<BorrowRecord> GetBorrowHistory();

        List<Book> SearchBook(string query);
    }

    internal class LibraryService : ILibraryService
    {
        private readonly IDatabase _db;
        private readonly IBookRepository _books;
        private readonly IMemberRepository _members;
        private readonly IBorrowRecordRepository _borrowRecords; 
        private readonly INotifier _notifier;

        public LibraryService(
            IDatabase db,
            IBookRepository books,
            IMemberRepository members,
            IBorrowRecordRepository borrowRecord,
            INotifier notifier)
        {
            _db = db;
            _books = books;
            _members = members;
            _borrowRecords = borrowRecord;
            _notifier = notifier;
        }

        public void BorrowBook(string bookId, string memberId)
        {
            var book   = _books.FindById(bookId);
            var member = _members.FindById(memberId);
        
            if (book == null)   { _notifier.Notify("Book not found.");   return; }
            if (member == null) { _notifier.Notify("Member not found."); return; }
        
            if (!book.IsAvailable())
            {
                _notifier.Notify($"'{book.Title}' is unavailable. Adding you to the waitlist.");
                _db.EnqueueWaitlist(bookId, memberId);   // LIST  — joins the queue
                return;
            }
        
            var dueDate = DateTime.UtcNow.AddDays(14);
        
            book.Borrow(memberId);
            // _books.Save(book);                                      // HASH  — updated book fields
            _db.TrackBorrowedByMember(memberId, bookId);            // SET   — member's active borrows
            _db.TrackDueDate(bookId, memberId, dueDate);            // SORTED SET — overdue index
            _db.PushBorrowRecord(new BorrowRecord(bookId, memberId)); // LIST — history log
            _db.IncrementBorrowScore(memberId);                     // SORTED SET — leaderboard
        
            _notifier.Notify($"'{book.Title}' borrowed. Due: {dueDate:dd MMM yyyy}");
        }


        public void AddBook(string title, string author)
        {
            if (string.IsNullOrWhiteSpace(title) || title.Length < 2)
            {
                _notifier.Notify("Invalid book title.");
                return;
            }

            if (string.IsNullOrWhiteSpace(author) || author.Length < 2)
            {
                _notifier.Notify("Invalid author name.");
                return;
            }

            var id = _db.GenerateId("books");
            var isbn = "N/A";
            var publishYear = DateTime.UtcNow.Year;
            var genres = new List<string>();

            var newBook = new Book(
                id,
                title,
                author,
                isbn,
                publishYear,
                genres
            );

            _books.Add(newBook);

            _notifier.Notify($"Book '{title}' added successfully.");
        }

        public void RegisterMember(string name, string membershipType)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                _notifier.Notify("Invalid member name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(membershipType) || membershipType.Length < 2)
            {
                _notifier.Notify("Invalid membership type.");
                return;
            }

            bool exists = _members.GetAll().Any(m =>
                m.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                m.MembershipType.Equals(membershipType, StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                _notifier.Notify($"Member '{name}' with membership type '{membershipType}' already exists.");
                return;
            }
            
            var id = _db.GenerateId("members");
            var newMember = new Member(id, name, membershipType);
            _members.Add(newMember);

            _notifier.Notify($"Member '{name}' registered successfully.");
        }

        public void ReturnBook(string bookId)
        {
            if (string.IsNullOrWhiteSpace(bookId) || bookId.Length < 2)
            {
                _notifier.Notify("Invalid book title.");
                return;
            }

            var book = _books.FindById(bookId);

            if (book == null) { _notifier.Notify("Book not found."); return; }

            if (book.IsAvailable()) { _notifier.Notify($"'{book.Title}' is not currently borrowed."); return; }

            book.Return();
        
            _notifier.Notify($"'{book.Title}' has been returned. Thank you!");
        }

        public List<Book> SearchBook(string query)
        {
            var result = new List<Book>();

            if (string.IsNullOrWhiteSpace(query)) return result;

            var books = _books.GetAll();

            foreach (var book in books)
            {
                if (book.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.Contains(query, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(book);
                }
            }

            return result;
        }

        List<Book> ILibraryService.GetAllBooks() => _books.GetAll();

        List<Member> ILibraryService.GetAllMembers() => _members.GetAll();

        List<BorrowRecord> ILibraryService.GetBorrowHistory() => _borrowRecords.GetAll();
    }



}
