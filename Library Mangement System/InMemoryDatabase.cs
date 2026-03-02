using Library_Management_System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal class InMemoryDatabase : IDatabase
    {
        public List<Book> Books { get; private set; } = new();
        public List<Member> Members { get; private set; } = new();
        public List<BorrowRecord> BorrowRecords { get; private set; } = new();

    }
}
