using Library_Management_System;
using Library_Mangement_System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System // correct to pascal case
{
    internal class Book : LibraryItem, IBorrowable, ISearchable
    {
        public string Author { get; }
        public string Isbn { get; }
        public int PublishYear { get; }
        public List<string> Genres { get; }

        public string? BorrowedByMemberId { get; private set; }

        public Book(string itemId, string title, string author,
                string isbn, int publishYear, List<string> genres,
                string? borrowedByMemberId = null)
        : base(itemId, title)
        {
            Author = author;
            Isbn = isbn;
            PublishYear = publishYear;
            Genres = genres;
            BorrowedByMemberId = borrowedByMemberId;
        }
    
        public override string GetSummary()
            => $"{Title} by {Author} ({PublishYear})";
    
        public override bool IsAvailable()
            => BorrowedByMemberId == null;
    
        public void Borrow(string memberId) => BorrowedByMemberId = memberId;
        public void Return() => BorrowedByMemberId = null;
    
        public string Search()
            => $"[{ItemId}] {Title} — {Author} | {(IsAvailable() ? "Available" : "Borrowed")}";

        }
}
