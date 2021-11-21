using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using VismaBookLibary.Models;

namespace VismaBookLibary.Repositories
{
    public class BookRepository : IBookRepository<Book>
    {
        public List<Book> GetAll()
        {
            var filePath = "../../../Data/books.json";
            var jsonString = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Book>>(jsonString);
        }

        public void Add(List<Book> books)
        {
            var booksString = JsonConvert.SerializeObject(books);
            File.WriteAllText(@"../../../Data/books.json", booksString);
        }

        public List<BorrowedBook> GetBorrowed()
        {
            var filePath = "../../../Data/borrowedbooks.json";
            var jsonString = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<BorrowedBook>>(jsonString);
        }

        public void AddBorrowed(string borrowedBook)
        {
            File.WriteAllText(@"../../../Data/borrowedbooks.json", borrowedBook);
        }
    }
}
