using Library_Management_System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Library_Management_System
{
    internal interface IBookRepository
    {
        void Add(Book book);
        Book? FindById(string id);
        List<Book> GetAll();

    }

    internal class BookRepository: IBookRepository
    {
        private readonly IDatabase _db;

        public BookRepository(IDatabase db)
        {
            _db = db;
        }

        public void Add(Book book) => _db.Books.Add(book);
        public Book? FindById(string id) => _db.Books.FirstOrDefault(b => b.ItemId == id);
        public List<Book> GetAll() => _db.Books;

    }
}
