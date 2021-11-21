using MoreLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VismaBookLibary.Models;
using VismaBookLibary.Models.DTO;
using VismaBookLibary.Repositories;

namespace VismaBookLibary.Services
{
    public class BookService : IBookRepository
    {
        private readonly IBookRepository<Book> _bookRepository;

        public BookService(IBookRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public string GetAll()
        {
            var books = _bookRepository.GetAll();

            var booksString = "";

            foreach (var book in books)
            {
                booksString += @$"
    Name: {book.Name}
    Author: {book.Author}
    Category: {book.Category}
    Language: {book.Language}
    Publication year: {book.PublicationYear}
    ISBN: {book.ISBN}
    Quantity: {book.Quantity}
";
            }

            return booksString;
        }

        public string GetFiltered(string filterDTOJson)
        {
            var books = _bookRepository.GetAll();

            var filterDTO = JsonConvert.DeserializeObject<FilterDTO>(filterDTOJson);

            var filteredBooks = new List<Book>();

            string booksString = "";

            foreach (var filter in filterDTO.Filters)
            {
                switch (filter.Split(" ", 2, StringSplitOptions.RemoveEmptyEntries)[0].ToLower())
                {
                    case "name":
                        filteredBooks.AddRange(books.Where(b => b.Name.ToLower() == filter.Split(" ", 2)[1].ToLower()).ToList());
                        break;

                    case "author":
                        filteredBooks.AddRange(books.Where(b => b.Author.ToLower() == filter.Split(" ", 2)[1].ToLower()).ToList());
                        break;

                    case "category":
                        filteredBooks.AddRange(books.Where(b => b.Category.ToLower() == filter.Split(" ", 2)[1].ToLower()).ToList());
                        break;

                    case "language":
                        filteredBooks.AddRange(books.Where(b => b.Language.ToLower() == filter.Split(" ", 2)[1].ToLower()).ToList());
                        break;

                    case "isbn":
                        filteredBooks.AddRange(books.Where(b => b.ISBN.ToLower() == filter.Split(" ", 2)[1].ToLower()).ToList());
                        break;

                    case "available":
                        filteredBooks.AddRange(books.Where(b => b.Available == true).ToList());
                        break;

                    default:
                        break;
                }
            }

            filteredBooks = filteredBooks.DistinctBy(b => b.ISBN).ToList();

            foreach (var book in filteredBooks)
            {
                booksString += @$"
    Name: {book.Name}
    Author: {book.Author}
    Category: {book.Category}
    Language: {book.Language}
    Publication year: {book.PublicationYear}
    ISBN: {book.ISBN}
    Quantity: {book.Quantity}
";
            }
            return booksString;
        }

        //Add

        public string Add(string bookDTOJson)
        {
            var books = _bookRepository.GetAll();

            var book = JsonConvert.DeserializeObject<Book>(bookDTOJson);

            var existingBook = books.FirstOrDefault(b => b.ISBN == book.ISBN);

            if (existingBook != null)
            {
                existingBook.Quantity += book.Quantity;
                _bookRepository.Add(books);
                return "We got that, but we will keep it. Thank You!";
            }

            books.Add(book);
            _bookRepository.Add(books);
            return "New book in our list. Thank You!";
        }

        //Take

        public string Take(string borrowDTOJson)
        {
            if (!CheckCustomerBeforBorrow(borrowDTOJson))
                return "You need to return one or more books, to borrow new ones.";

            if (!CheckIfBookExist(borrowDTOJson))
                return "Sorry we can't find that book";

            if (!CheckBookIsAvailable(borrowDTOJson))
                return "Sorry no more books awailable to borrow.";

            LendBook(borrowDTOJson);

            return "Book is yours. :)";
        }

        public bool CheckCustomerBeforBorrow(string borrowDTOJson)
        {
            var borrowDTO = JsonConvert.DeserializeObject<BorrowDTO>(borrowDTOJson);

            var allBorrowedBooks = _bookRepository.GetBorrowed();

            var customerBooksCount = allBorrowedBooks.Where(c => c.CustomerId == borrowDTO.CustomerId).Count();

            if (customerBooksCount >= 3)
                return false;

            return true;
        }

        public bool CheckIfBookExist(string borrowDTOJson)
        {
            var borrowDTO = JsonConvert.DeserializeObject<BorrowDTO>(borrowDTOJson);

            var books = _bookRepository.GetAll();

            var book = books.FirstOrDefault(b => b.ISBN == borrowDTO.ISBN);

            if (book == null)
                return false;

            return true;
        }

        public bool CheckBookIsAvailable(string borrowDTOJson)
        {
            var borrowDTO = JsonConvert.DeserializeObject<BorrowDTO>(borrowDTOJson);

            var books = _bookRepository.GetAll();

            var book = books.FirstOrDefault(b => b.ISBN == borrowDTO.ISBN);

            if (book.Quantity < 2 || book.Available == false)
                return false;

            book.Quantity -= 1;

            if (book.Quantity < 2)
                book.Available = false;

            _bookRepository.Add(books);

            return true;
        }

        public void LendBook(string borrowDTOJson)
        {
            var borrowDTO = JsonConvert.DeserializeObject<BorrowDTO>(borrowDTOJson);

            var allBorrowedBooks = _bookRepository.GetBorrowed();

            var borrowedBook = new BorrowedBook()
            {
                ISBN = borrowDTO.ISBN,
                CustomerId = borrowDTO.CustomerId,
                BorrowEnds = DateTime.Today.AddDays(Int32.Parse(borrowDTO.Weeks) * 7)
            };

            allBorrowedBooks.Add(borrowedBook);

            _bookRepository.AddBorrowed(JsonConvert.SerializeObject(allBorrowedBooks));
        }

        //Return

        public string Return(string returnDTOJson)
        {
            if (!DoBorrowExist(returnDTOJson))
                return "Sorry this book belongs to another library.";

            var late = false;

            if (!ReturnLate(returnDTOJson))
                late = true;

            RemoveFromBorrowedBooks(returnDTOJson);

            AddReturnedBook(returnDTOJson);

            if (late)
                return "Mistah, you read very slow.";

            return "Thank you, come again.";
        }

        public bool DoBorrowExist(string returnDTOJson)
        {
            var returnDTO = JsonConvert.DeserializeObject<ReturnDTO>(returnDTOJson);

            var allBorrowedBooks = _bookRepository.GetBorrowed();

            var customerBook = allBorrowedBooks.FirstOrDefault(b => b.ISBN == returnDTO.ISBN && b.CustomerId == returnDTO.CustomerId);

            if (customerBook == null)
                return false;

            return true;
        }

        public void RemoveFromBorrowedBooks(string returnDTOJson)
        {
            var returnDTO = JsonConvert.DeserializeObject<ReturnDTO>(returnDTOJson);

            var allBorrowedBooks = _bookRepository.GetBorrowed();

            var customerBook = allBorrowedBooks.FirstOrDefault(b => b.ISBN == returnDTO.ISBN && b.CustomerId == returnDTO.CustomerId);

            allBorrowedBooks.Remove(customerBook);

            _bookRepository.AddBorrowed(JsonConvert.SerializeObject(allBorrowedBooks));
        }

        public void AddReturnedBook(string returnDTOJson)
        {
            var returnDTO = JsonConvert.DeserializeObject<ReturnDTO>(returnDTOJson);

            var books = _bookRepository.GetAll();

            var book = books.FirstOrDefault(b => b.ISBN == returnDTO.ISBN);

            book.Quantity += 1;

            books.Add(book);

            _bookRepository.Add(books);
        }

        public bool ReturnLate(string returnDTOJson)
        {
            var returnDTO = JsonConvert.DeserializeObject<ReturnDTO>(returnDTOJson);

            var allBorrowedBooks = _bookRepository.GetBorrowed();

            var customerBook = allBorrowedBooks.FirstOrDefault(b => b.ISBN == returnDTO.ISBN && b.CustomerId == returnDTO.CustomerId);

            if (customerBook.BorrowEnds < DateTime.Today)
                return false;

            return true;
        }

        //Delete

        public string Delete(string deleteDTOJson)
        {
            if (!CheckIfBookExistForDelete(deleteDTOJson))
                return "Book instance does't exist.";

            if (!CheckIfBookIsBorrowed(deleteDTOJson))
                return $"There are customers who hold book with  given ISBN.";

            RemoveBook(deleteDTOJson);

            return "Book deleted.";
        }

        public bool CheckIfBookExistForDelete(string deleteDTOJson)
        {
            var deleteDTO = JsonConvert.DeserializeObject<DeleteDTO>(deleteDTOJson);

            var books = _bookRepository.GetAll();

            var bookToRemove = books.FirstOrDefault(b => b.ISBN == deleteDTO.ISBN);

            if (bookToRemove == null)
                return false;

            return true;
        }

        public bool CheckIfBookIsBorrowed(string deleteDTOJson)
        {
            var deleteDTO = JsonConvert.DeserializeObject<DeleteDTO>(deleteDTOJson);

            var allBorrowedBooks = _bookRepository.GetBorrowed();

            var borrowedBooks = allBorrowedBooks.Any(bb => bb.ISBN == deleteDTO.ISBN);

            if (borrowedBooks)
                return false;

            return true;
        }

        public void RemoveBook(string deleteDTOJson)
        {
            var deleteDTO = JsonConvert.DeserializeObject<DeleteDTO>(deleteDTOJson);

            var books = _bookRepository.GetAll();

            var bookToRemove = books.FirstOrDefault(b => b.ISBN == deleteDTO.ISBN);

            books.Remove(bookToRemove);
            _bookRepository.Add(books);
        }



    }
}