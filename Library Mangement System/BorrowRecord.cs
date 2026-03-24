using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal record BorrowRecord
    {
        public string BookId { get; }
        public string MemberId { get; }
        public DateTime BorrowedOn { get; }
        public DateTime DueDate { get; }

        public BorrowRecord(string bookId, string memberId)
        {
            BookId = bookId;
            MemberId = memberId;
            BorrowedOn = DateTime.Now;
            DueDate = DateTime.Now.AddDays(14);
        }
    }
}
