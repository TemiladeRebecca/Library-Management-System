using Library_Management_System;

namespace Library_Management_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //List<Person> list = new List<Person>
            //{
            //    new Member("2", "Bob", "Premium"),
            //    new Librarian("3", "Charlie", "EMP123")
            //};

            //foreach (var person in list)
            //{
            //    Console.WriteLine(person.GetDetails());
            //}

            // Wire up the dependency graph manually
            IDatabase database = new InMemoryDatabase();
            INotifier notifier = new ConsoleNotifier();
            IBookRepository books = new BookRepository(database);
            IMemberRepository members = new MemberRepository(database);
            IBorrowRecordRepository borrowRecords = new BorrowRecordRepository(database);
            ILibraryService service = new LibraryService(books, members, borrowRecords, notifier);
            IMenuHandler menu = new MenuHandler(service, notifier);

            menu.Run();

        }


        public interface IBorrowable
        {
            void Borrow(string memberId);
            void Return();
        }

        public interface ISearchable
        {
            string Search();
        }

    }
}
