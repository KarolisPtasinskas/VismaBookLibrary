using Microsoft.Extensions.DependencyInjection;
using System;
using VismaBookLibary.Controllers;
using VismaBookLibary.Models;
using VismaBookLibary.Repositories;
using VismaBookLibary.Services;

namespace VismaBookLibary
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var vismaLibrary = serviceProvider.GetService<App>();

            UserInterface ui = new();

            Console.WriteLine(ui.Logo);
            Console.WriteLine(ui.Greeting);

            while (true)
            {
                var commandInput = Console.ReadLine();

                if (commandInput == "help")
                {
                    Console.WriteLine(ui.Help);
                    continue;
                };

                if (commandInput == "exit")
                {
                    break;
                };

                var commandValidator = new CommandValidator(commandInput);

                if (commandValidator.Error)
                {
                    foreach (var error in commandValidator.ErrorMessages)
                    {
                        Console.WriteLine("- " + error);

                    }
                    continue;
                }

                vismaLibrary.Run(commandInput);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IBookRepository<Book>, BookRepository>();

            services.AddTransient<BookService>();

            services.AddTransient<BooksController>();

            services.AddTransient<App>();
        }
    }
}