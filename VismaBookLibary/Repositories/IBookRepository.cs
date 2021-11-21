using System.Collections.Generic;
using VismaBookLibary.Models;

namespace VismaBookLibary.Repositories
{
    public interface IBookRepository<T> where T : class
    {
        void Add(List<Book> books);
        void AddBorrowed(string borrowedBook);
        List<Book> GetAll();
        List<BorrowedBook> GetBorrowed();
    }
}