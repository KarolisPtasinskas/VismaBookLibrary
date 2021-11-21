using System;
using System.Collections.Generic;
using System.Linq;

namespace VismaBookLibary.Models
{
    class CommandValidator : AppCommands
    {
        public CommandValidator(string userInput)
        {
            ValidateInput(userInput);
        }

        public bool Error { get; set; } = false;
        public List<string> ErrorMessages { get; set; } = new();

        private void ValidateInput(string userInput)
        {
            var obj = userInput.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].ToLower();

            if (!Objects.Contains(obj))
            {
                Error = true;
                ErrorMessages.Add($"Object '{obj}' does't exist. Please write 'help' for command list.");
                return;
            }

            if (userInput.Split(" ").Count() == 2)
            {
                var mainCommand = userInput.Split(" ", StringSplitOptions.RemoveEmptyEntries)[1].ToLower();

                ValidateMainCommand(obj, mainCommand);

            }

            if (userInput.Split(" ").Count() > 2)
            {
                Error = true;
                ErrorMessages.Add($"{obj} can handle only one command.");
                return;

            }
        }

        private void ValidateMainCommand(string obj, string mainCommand)
        {
            switch (obj)
            {
                case "books":
                    if (!BookMainCommands.Contains(mainCommand))
                    {
                        Error = true;
                        ErrorMessages.Add($"Object 'books' does't have command '{mainCommand}'. Please write 'help' for command list.");
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
