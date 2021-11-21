using VismaBookLibary.Services;

namespace VismaBookLibary.Controllers
{
    class BooksController
    {
        private readonly BookService _bookService;
        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }
        public string GetAll()
        {
            return _bookService.GetAll();
        }

        public string GetFiltered(string additionalInfo)
        {
            return _bookService.GetFiltered(additionalInfo);
        }

        public string Add(string additionalInfo)
        {
            return _bookService.Add(additionalInfo);
        }

        public string Take(string additionalInfo)
        {
            return _bookService.Take(additionalInfo);
        }

        public string Return(string additionalInfo)
        {
            return _bookService.Return(additionalInfo);
        }

        public string Delete(string additionalInfo)
        {
            return _bookService.Delete(additionalInfo);
        }
    }
}
