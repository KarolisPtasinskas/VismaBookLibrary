namespace VismaBookLibary
{
    class UserInterface
    {

        public string Logo { get; }
        public string Greeting { get; }
        public string Help { get; set; }


        public UserInterface()
        {
            Logo = @"
             __      ___                       _      _ _                          
             \ \    / (_)                     | |    (_) |                         
              \ \  / / _ ___ _ __ ___   __ _  | |     _| |__  _ __ __ _ _ __ _   _ 
               \ \/ / | / __| '_ ` _ \ / _` | | |    | | '_ \| '__/ _` | '__| | | |
                \  /  | \__ \ | | | | | (_| | | |____| | |_) | | | (_| | |  | |_| |
                 \/   |_|___/_| |_| |_|\__,_| |______|_|_.__/|_|  \__,_|_|   \__, |
                                                                              __/ |
                                                                              |___/
";

            Greeting = "Wellcome Outsider. Please specify what do you want or call customer support by writing 'help'.\n";

            Help = @$"The program uses a command architecture:
 - [Object] [Command]

Where
 - [Object]: books
 - [Commands]: list, filter, add, take, return, delete

Additional:
 - [filter] attributes: *name BOOK NAME, *author BOOK AUTHOR, *category BOOK CATEGORY, *language BOOK LANGUAGE, *isbn BOOK ISBN, *available

Example:
 - book list               - returns list of all books
 - book filter
    - *author J.K.Rowling  - returns list of books where author is J.K.Rowling (more than one filter is possible)
 - book add                - starts adding book with folowing info.

All additional information will be collected through questions. Don't worry, you won't have to think anything by yourself.
";


        }

    }
}
