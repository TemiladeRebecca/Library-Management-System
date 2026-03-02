using Library_Management_System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal interface IDatabase
    {
        List<Book> Books { get; }
        List<Member> Members { get; }
        List<BorrowRecord> BorrowRecords { get; }
    }
}
