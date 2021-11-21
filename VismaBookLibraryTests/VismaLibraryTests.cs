using AutoFixture;
using FluentAssertions;
using Moq;
using MoreLinq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using VismaBookLibary.Models;
using VismaBookLibary.Models.DTO;
using VismaBookLibary.Repositories;
using VismaBookLibary.Services;

namespace BookServiceTest
{
    public class Tests
    {
        [Test]
        public void BooksService_GetAll_Returns_ListOfBookString()
        {
            var fixture = new Fixture();
            var books = fixture.CreateMany<Book>().ToList();

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

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

            var result = bookService.GetAll();

            result.Should().BeEquivalentTo(booksString);
        }

        [Test]
        public void BooksService_Add_Returns_NewBookMessage()
        {
            var fixture = new Fixture();
            var newBookDTO = fixture.Create<BookDTO>();
            var newBookDTOJson = JsonConvert.SerializeObject(newBookDTO);
            var book = JsonConvert.DeserializeObject<Book>(newBookDTOJson);

            var books = fixture.CreateMany<Book>().ToList();

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            var result = bookService.Add(newBookDTOJson);

            result.Should().BeEquivalentTo("New book in our list. Thank You!");
        }

        [Test]
        public void BooksService_Add_Returns_WeGotThatMessage()
        {
            var fixture = new Fixture();
            var newBookDTO = fixture.Create<BookDTO>();
            var newBookDTOJson = JsonConvert.SerializeObject(newBookDTO);
            var book = JsonConvert.DeserializeObject<Book>(newBookDTOJson);

            var books = fixture.CreateMany<Book>().ToList();

            books.Add(book);

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            var result = bookService.Add(newBookDTOJson);

            result.Should().BeSameAs("We got that, but we will keep it. Thank You!");
        }

        [Test]
        public void BooksService_CheckCustomerBeforBorrow_Returns_False()
        {
            var fixture = new Fixture();
            var allBorrowedBooks = fixture.CreateMany<BorrowedBook>().ToList();
            allBorrowedBooks[0].CustomerId = "Testeris";
            allBorrowedBooks[1].CustomerId = "Testeris";
            allBorrowedBooks[2].CustomerId = "Testeris";

            var borrowDTOJson = $"{{\"Weeks\":\"8\",\"CustomerId\":\"Testeris\",\"ISBN\":\"5522336699\"}}";
            var borrowDTO = JsonConvert.DeserializeObject<BorrowDTO>(borrowDTOJson);

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetBorrowed()).Returns(allBorrowedBooks);
            var bookService = new BookService(mockBookRepository.Object);

            var result = bookService.CheckCustomerBeforBorrow(borrowDTOJson);

            result.Should().Be(false);
        }

        [Test]
        public void BooksService_CheckIfBookExist_Returns_False()
        {
            var fixture = new Fixture();
            var books = fixture.CreateMany<Book>().ToList();
            var borrowDTOJson = $"{{\"Weeks\":\"8\",\"CustomerId\":\"Testeris\",\"ISBN\":\"9999\"}}";

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            var result = bookService.CheckIfBookExist(borrowDTOJson);

            result.Should().Be(false);
        }

        [Test]
        public void BooksService_CheckBookIsAvailable_Returns_False()
        {
            var fixture = new Fixture();
            var books = fixture.CreateMany<Book>().ToList();

            var borrowDTOJson = $"{{\"Weeks\":\"8\",\"CustomerId\":\"Testeris\",\"ISBN\":\"5522336699\"}}";
            var borrowDTO = JsonConvert.DeserializeObject<BorrowDTO>(borrowDTOJson);

            books[0].Available = false;
            books[0].ISBN = borrowDTO.ISBN;

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            var result = bookService.CheckBookIsAvailable(borrowDTOJson);
            result.Should().Be(false);
        }

        [Test]
        public void BooksService_LendBook_Returns_NoNull()
        {
            var fixture = new Fixture();
            var newBorrowedBook = fixture.Create<BorrowedBook>();
            var allBorrowedBooks = fixture.CreateMany<BorrowedBook>().ToList();
            var borrowDTOJson = $"{{\"Weeks\":\"8\",\"CustomerId\":\"Testeris\",\"ISBN\":\"9999\"}}";
            var borrowDTO = JsonConvert.DeserializeObject<BorrowDTO>(borrowDTOJson);

            newBorrowedBook.ISBN = borrowDTO.ISBN;

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetBorrowed()).Returns(allBorrowedBooks);
            var bookService = new BookService(mockBookRepository.Object);

            bookService.LendBook(borrowDTOJson);

            var addedBook = allBorrowedBooks.FirstOrDefault(b => b.ISBN == newBorrowedBook.ISBN);

            addedBook.Should().NotBeNull();
        }

        [Test]
        public void BooksService_DoBorrowExist_Returns_False()
        {
            var fixture = new Fixture();
            var allBorrowedBooks = fixture.CreateMany<BorrowedBook>().ToList();
            var returnDTOJson = $"{{\"CustomerId\":\"KARPTA1111\",\"ISBN\":\"1111155555\"}}";

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetBorrowed()).Returns(allBorrowedBooks);
            var bookService = new BookService(mockBookRepository.Object);

            var result = bookService.DoBorrowExist(returnDTOJson);
            result.Should().Be(false);
        }

        [Test]
        public void BooksService_RemoveFromBorrowedBooks_Returns_Null()
        {
            var fixture = new Fixture();
            var borrowedBook = fixture.Create<BorrowedBook>();
            var allBorrowedBooks = fixture.CreateMany<BorrowedBook>().ToList();
            var returnDTOJson = $"{{\"CustomerId\":\"KARPTA1111\",\"ISBN\":\"1111155555\"}}";
            var returnDTO = JsonConvert.DeserializeObject<BorrowDTO>(returnDTOJson);

            allBorrowedBooks[0].CustomerId = returnDTO.CustomerId;
            allBorrowedBooks[0].ISBN = returnDTO.ISBN;
            borrowedBook.ISBN = returnDTO.ISBN;

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetBorrowed()).Returns(allBorrowedBooks);
            var bookService = new BookService(mockBookRepository.Object);

            bookService.RemoveFromBorrowedBooks(returnDTOJson);

            var removedBook = allBorrowedBooks.FirstOrDefault(b => b.ISBN == borrowedBook.ISBN);

            removedBook.Should().BeNull();
        }

        [Test]
        public void BooksService_AddReturnedBook_Returns_NotNull()
        {
            var fixture = new Fixture();
            var books = fixture.CreateMany<Book>().ToList();
            var returnedBook = fixture.Create<Book>();
            var returnDTOJson = $"{{\"CustomerId\":\"KARPTA1111\",\"ISBN\":\"0439136369\"}}";
            var returnDTO = JsonConvert.DeserializeObject<BorrowDTO>(returnDTOJson);

            books[0].ISBN = returnDTO.ISBN;
            returnedBook.ISBN = returnDTO.ISBN;

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            bookService.AddReturnedBook(returnDTOJson);

            var addedBook = books.FirstOrDefault(b => b.ISBN == returnedBook.ISBN);

            addedBook.Should().NotBeNull();
        }


        [Test]
        public void BooksService_CheckIfBookExistForDelete_Returns_False()
        {
            var fixture = new Fixture();
            var books = fixture.CreateMany<Book>().ToList();
            var deleteDTOJson = $"{{\"ISBN\":\"333777999\"}}";

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            var result = bookService.CheckIfBookExistForDelete(deleteDTOJson);

            result.Should().Be(false);
        }

        [Test]
        public void BooksService_CheckIfBookIsBorrowed_Returns_False()
        {
            var fixture = new Fixture();
            var allBorrowedBooks = fixture.CreateMany<BorrowedBook>().ToList();
            var deleteDTOJson = $"{{\"ISBN\":\"333777999\"}}";
            var deleteDTO = JsonConvert.DeserializeObject<BorrowDTO>(deleteDTOJson);

            allBorrowedBooks[0].ISBN = deleteDTO.ISBN;

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetBorrowed()).Returns(allBorrowedBooks);
            var bookService = new BookService(mockBookRepository.Object);

            var result = bookService.CheckIfBookIsBorrowed(deleteDTOJson);
            result.Should().Be(false);
        }

        [Test]
        public void BooksService_RemoveBook_Returns_Null()
        {
            var fixture = new Fixture();
            var removeBook = fixture.Create<Book>();
            var books = fixture.CreateMany<Book>().ToList();
            var deleteDTOJson = $"{{\"ISBN\":\"111555111\"}}";
            var deleteDTO = JsonConvert.DeserializeObject<DeleteDTO>(deleteDTOJson);

            removeBook.ISBN = deleteDTO.ISBN;
            books.Add(removeBook);

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            bookService.RemoveBook(deleteDTOJson);

            var deletedBook = books.FirstOrDefault(b => b.ISBN == removeBook.ISBN);

            deletedBook.Should().BeNull();


        }

        [Test]
        public void BooksService_GetFiltered_Returns_CategoryFilterListString()
        {
            var fixture = new Fixture();
            var books = fixture.CreateMany<Book>().ToList();

            var filterDTOJson = $"{{\"Filters\":[\"category fantasy\"]}}";
            var filterDTO = JsonConvert.DeserializeObject<FilterDTO>(filterDTOJson);

            books[0].Category = filterDTO.Filters[0].Split(" ", 2)[1].ToLower();

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            var filteredBooks = new List<Book>();

            string booksString = "";

            foreach (var filter in filterDTO.Filters)
            {
                switch ("category")
                {
                    case "category":
                        filteredBooks.AddRange(books.Where(b => b.Category.ToLower() == "fantasy").ToList());
                        break;
                }
            }

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

            var result = bookService.GetFiltered(filterDTOJson);

            result.Should().BeEquivalentTo(booksString);
        }

        [Test]
        public void BooksService_GetFiltered_Returns_NameFilterListString()
        {
            var fixture = new Fixture();
            var books = fixture.CreateMany<Book>().ToList();

            var filterDTOJson = $"{{\"Filters\":[\"name fantasy\"]}}";
            var filterDTO = JsonConvert.DeserializeObject<FilterDTO>(filterDTOJson);

            books[0].Name = filterDTO.Filters[0].Split(" ", 2)[1].ToLower();

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            var filteredBooks = new List<Book>();

            string booksString = "";

            foreach (var filter in filterDTO.Filters)
            {
                switch ("name")
                {
                    case "name":
                        filteredBooks.AddRange(books.Where(b => b.Name.ToLower() == "fantasy").ToList());
                        break;
                }
            }

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

            var result = bookService.GetFiltered(filterDTOJson);

            result.Should().BeEquivalentTo(booksString);
        }

        [Test]
        public void BooksService_GetFiltered_Returns_AuthorFilterListString()
        {
            var fixture = new Fixture();
            var books = fixture.CreateMany<Book>().ToList();

            var filterDTOJson = $"{{\"Filters\":[\"author fantasy\"]}}";
            var filterDTO = JsonConvert.DeserializeObject<FilterDTO>(filterDTOJson);

            books[0].Author = filterDTO.Filters[0].Split(" ", 2)[1].ToLower();

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            var filteredBooks = new List<Book>();

            string booksString = "";

            foreach (var filter in filterDTO.Filters)
            {
                switch ("author")
                {
                    case "author":
                        filteredBooks.AddRange(books.Where(b => b.Author.ToLower() == "fantasy").ToList());
                        break;
                }
            }

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

            var result = bookService.GetFiltered(filterDTOJson);

            result.Should().BeEquivalentTo(booksString);
        }

        [Test]
        public void BooksService_GetFiltered_Returns_LanguageFilterListString()
        {
            var fixture = new Fixture();
            var books = fixture.CreateMany<Book>().ToList();

            var filterDTOJson = $"{{\"Filters\":[\"language fantasy\"]}}";
            var filterDTO = JsonConvert.DeserializeObject<FilterDTO>(filterDTOJson);

            books[0].Language = filterDTO.Filters[0].Split(" ", 2)[1].ToLower();

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            var filteredBooks = new List<Book>();

            string booksString = "";

            foreach (var filter in filterDTO.Filters)
            {
                switch ("language")
                {
                    case "language":
                        filteredBooks.AddRange(books.Where(b => b.Language.ToLower() == "fantasy").ToList());
                        break;
                }
            }

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

            var result = bookService.GetFiltered(filterDTOJson);

            result.Should().BeEquivalentTo(booksString);
        }


        [Test]
        public void BooksService_GetFiltered_Returns_IsbnFilterListString()
        {
            var fixture = new Fixture();
            var books = fixture.CreateMany<Book>().ToList();

            var filterDTOJson = $"{{\"Filters\":[\"isbn fantasy\"]}}";
            var filterDTO = JsonConvert.DeserializeObject<FilterDTO>(filterDTOJson);

            books[0].ISBN = filterDTO.Filters[0].Split(" ", 2)[1].ToLower();

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            var filteredBooks = new List<Book>();

            string booksString = "";

            foreach (var filter in filterDTO.Filters)
            {
                switch ("isbn")
                {
                    case "isbn":
                        filteredBooks.AddRange(books.Where(b => b.ISBN.ToLower() == "fantasy").ToList());
                        break;
                }
            }

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

            var result = bookService.GetFiltered(filterDTOJson);

            result.Should().BeEquivalentTo(booksString);
        }

        [Test]
        public void BooksService_GetFiltered_Returns_AvailableFilterListString()
        {
            var fixture = new Fixture();
            var books = fixture.CreateMany<Book>().ToList();

            var filterDTOJson = $"{{\"Filters\":[\"available\"]}}";
            var filterDTO = JsonConvert.DeserializeObject<FilterDTO>(filterDTOJson);

            books[0].Available = true;
            books[1].Available = false;
            books[2].Available = false;

            var mockBookRepository = new Mock<IBookRepository<Book>>();
            mockBookRepository.Setup(b => b.GetAll()).Returns(books);
            var bookService = new BookService(mockBookRepository.Object);

            var filteredBooks = new List<Book>();

            string booksString = "";

            foreach (var filter in filterDTO.Filters)
            {
                switch ("available")
                {
                    case "available":
                        filteredBooks.AddRange(books.Where(b => b.Available == true).ToList());
                        break;
                }
            }

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

            var result = bookService.GetFiltered(filterDTOJson);

            result.Should().BeEquivalentTo(booksString);
        }
    }
}