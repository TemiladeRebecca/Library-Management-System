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
    }

    internal class LibraryService : ILibraryService
    {
        private readonly IBookRepository _books;
        private readonly IMemberRepository _members;
        private readonly IBorrowRecordRepository _borrowRecords; 
        private readonly INotifier _notifier;

        public LibraryService(
            IBookRepository books,
            IMemberRepository members,
            IBorrowRecordRepository borrowRecord,
            INotifier notifier)
        {
            _books = books;
            _members = members;
            _borrowRecords = borrowRecord;
            _notifier = notifier;
        }

        public void BorrowBook(string bookId, string memberId)
        {
            var book = _books.FindById(bookId);
            var member = _members.FindById(memberId);

            if (book == null) { _notifier.Notify("Book not found."); return; }
            if (member == null) { _notifier.Notify("Member not found."); return; }
            if (!book.IsAvailable()) { _notifier.Notify($"'{book.Title}' is already borrowed."); return; }

            book.Borrow(memberId);
            var record = new BorrowRecord(bookId, memberId);
            _borrowRecords.Add(record);
            _notifier.Notify($"'{book.Title}' borrowed by {member.Name}. Due: {record.DueDate:dd MMM yyyy}");
        }

        // Implement all other interface methods...
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
            var newBook = new Book(author, Guid.NewGuid().ToString(), title);

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

            var newMember = new Member(Guid.NewGuid().ToString(), name, membershipType);
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

        List<Book> ILibraryService.GetAllBooks() => _books.GetAll();

        List<Member> ILibraryService.GetAllMembers() => _members.GetAll();

        List<BorrowRecord> ILibraryService.GetBorrowHistory() => _borrowRecords.GetAll();
    }



}
