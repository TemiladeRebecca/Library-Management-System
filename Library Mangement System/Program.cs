using Library_Management_System;

namespace Library_Management_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IDatabase database = new RedisDatabase("localhost:6379");
            INotifier notifier = new ConsoleNotifier();
            IBookRepository books = new BookRepository(database);
            IMemberRepository members = new MemberRepository(database);
            IBorrowRecordRepository borrowRecords = new BorrowRecordRepository(database);
            ILibraryService service = new LibraryService(database, books, members, borrowRecords, notifier);
            IMenuHandler menu = new MenuHandler(service, notifier);

            menu.Run();

        }
    }
}
