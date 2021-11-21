using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using VismaBookLibary.Models;
using VismaBookLibary.Models.DTO;
using VismaBookLibary.Validators;

namespace VismaBookLibary
{
    public static class SecondaryDataService
    {
        public static string GetFilterDetails()
        {
            Console.WriteLine();

            var filterDTO = new FilterDTO();

            var question = @"Please add filter attributes (e.g. *author AUTHOR NAME *category CATEGORY).

Possible filters: *name, *author, *category, *language, *isbn, *available";
            string[] separators = { "*", " *" };
            string input;

            Console.WriteLine(question);
            input = Console.ReadLine();

            var filteres = new List<Book>();

            var filters = input.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();

            filterDTO.Filters = filters;

            string filterDTOJson = JsonConvert.SerializeObject(filterDTO);

            return filterDTOJson;
        }

        public static string GetBookDetails()
        {
            Console.WriteLine();

            var bookDTO = new BookDTO();

            var questions = new List<string> { "Name: ", "Author: ", "Category: ", "Language: ", "Publication year: ", "ISBN: ", "Quantity: " };
            string input;

            Console.Write(questions[0]);
            input = Console.ReadLine();
            bookDTO.Name = input;

            Console.Write(questions[1]);
            input = Console.ReadLine();
            bookDTO.Author = input;

            Console.Write(questions[2]);
            input = Console.ReadLine();
            bookDTO.Category = input;

            Console.Write(questions[3]);
            input = Console.ReadLine();
            bookDTO.Language = input;

            Console.Write(questions[4]);
            input = Console.ReadLine();
            bookDTO.PublicationYear = Int32.Parse(input);

            Console.Write(questions[5]);
            input = Console.ReadLine();
            bookDTO.ISBN = input;

            Console.Write(questions[6]);
            input = Console.ReadLine();
            bookDTO.Quantity = Int32.Parse(input);

            string bookDTOJson = JsonConvert.SerializeObject(bookDTO);

            return bookDTOJson;
        }

        public static string GetBorrowDetails()
        {
            Console.WriteLine();

            var borrowDTO = new BorrowDTO();

            var questions = new List<string> { "How long do you need a book (specify weeks): ", "Please enter your CustomerId: ", "Please enter book ISBN code: " };
            var input = "";
            var weeksValid = false;

            while (weeksValid == false)
            {
                Console.Write(questions[0]);
                input = Console.ReadLine();

                var isValid = SecondaryInfoValidator.CheckLendPeriod(input);

                if (!isValid)
                {
                    Console.WriteLine("Sorry we can't read input or lend period does not meet requirements (min. 1, max 8 weeks).");
                    continue;
                }
                weeksValid = true;
            }
            borrowDTO.Weeks = input;


            Console.Write(questions[1]);
            input = Console.ReadLine();
            borrowDTO.CustomerId = input;

            Console.Write(questions[2]);
            input = Console.ReadLine();
            borrowDTO.ISBN = input;

            string borrowDTOJson = JsonConvert.SerializeObject(borrowDTO);

            return borrowDTOJson;
        }

        public static string GetReturnDetails()
        {
            Console.WriteLine();

            var returnDTO = new ReturnDTO();

            var questions = new List<string> { "Please enter your CustomerId: ", "Please enter the ISBN code of the returned book: " };
            string input;

            Console.Write(questions[0]);
            input = Console.ReadLine();
            returnDTO.CustomerId = input;

            Console.Write(questions[1]);
            input = Console.ReadLine();
            returnDTO.ISBN = input;

            string returnDTOJson = JsonConvert.SerializeObject(returnDTO);

            return returnDTOJson;
        }

        public static string GetDeleteDetails()
        {
            Console.WriteLine();

            var deleteDTO = new DeleteDTO();

            var questions = new List<string> { "Enter the ISB code for the book you are deleting: " };
            string input;

            Console.Write(questions[0]);
            input = Console.ReadLine();
            deleteDTO.ISBN = input;

            string deleteDTOJson = JsonConvert.SerializeObject(deleteDTO);

            return deleteDTOJson;
        }
    }
}
