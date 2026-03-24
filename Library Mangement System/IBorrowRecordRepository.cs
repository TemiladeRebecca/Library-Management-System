using Library_Management_System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal interface IBorrowRecordRepository
    {
        void Add(BorrowRecord record);
        List<BorrowRecord> GetAll();
    }

    internal class BorrowRecordRepository : IBorrowRecordRepository
    {
        private readonly IDatabase _db;
        public BorrowRecordRepository(IDatabase db)
        {
            _db = db;
        }
        public void Add(BorrowRecord record) => _db.PushBorrowRecord(record);
        public List<BorrowRecord> GetAll() => _db.GetBorrowHistory();
    }
}
