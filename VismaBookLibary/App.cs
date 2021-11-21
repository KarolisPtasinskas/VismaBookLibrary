using System;
using VismaBookLibary.Controllers;

namespace VismaBookLibary
{
    class App
    {
        private readonly BooksController booksController;
        public App(BooksController booksController)
        {
            this.booksController = booksController;
        }
        public void Run(string commandInput)
        {
            var additionalInfo = GetSecondaryInfo(commandInput);
            var result = TakeAction(commandInput, additionalInfo);

            Console.WriteLine(result);
        }
        public string GetSecondaryInfo(string commandInput)
        {
            var mainCommand = commandInput.Split(" ", StringSplitOptions.RemoveEmptyEntries)[1].ToLower();

            return mainCommand switch
            {
                "filter" => SecondaryDataService.GetFilterDetails(),
                "add" => SecondaryDataService.GetBookDetails(),
                "take" => SecondaryDataService.GetBorrowDetails(),
                "return" => SecondaryDataService.GetReturnDetails(),
                "delete" => SecondaryDataService.GetDeleteDetails(),
                _ => null,
            };
        }
        public string TakeAction(string commandInput, string additionalInfo)
        {
            var mainCommand = commandInput.Split(" ", StringSplitOptions.RemoveEmptyEntries)[1].ToLower();

            return mainCommand switch
            {
                "list" => booksController.GetAll(),
                "filter" => booksController.GetFiltered(additionalInfo),
                "add" => booksController.Add(additionalInfo),
                "take" => booksController.Take(additionalInfo),
                "return" => booksController.Return(additionalInfo),
                "delete" => booksController.Delete(additionalInfo),
                _ => null,
            };
        }

    }
}
