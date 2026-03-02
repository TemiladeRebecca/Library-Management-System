using Library_Management_System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal class Book : LibraryItem, Program.IBorrowable, Program.ISearchable
    {
        public string Author { get; }
        public string? BorrowedByMemberId { get; private set; }

        public Book (string author, string itemId, string title) : base(itemId, title) 
        {
            Author = author;
        }

        // From LibraryItem (abstract — must implement)
        public override string GetSummary() => $"{Title} by {Author}";
        public override bool IsAvailable() => BorrowedByMemberId == null;

        // From IBorrowable
        public void Borrow(string memberId) => BorrowedByMemberId = memberId;
        public void Return() => BorrowedByMemberId = null;

        // From ISearchable
        public string Search() => $"[{ItemId}] {Title} — {Author} | {(IsAvailable() ? "Available" : "Borrowed")}";



    }
}
