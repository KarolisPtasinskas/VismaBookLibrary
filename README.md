# VismaBookLibrary

Home work for Visma intership

The program uses a command architecture:

- {Object} {Command}

Where

- {Objects}: books
- {Commands}: list, filter, add, take, return, delete

Additional:

- {filter} attributes: *name BOOK NAME, *author BOOK AUTHOR, *category BOOK CATEGORY, *language BOOK LANGUAGE, *isbn BOOK ISBN, *available

Example:

- book list - returns list of all books
- book filter
- \*author J.K.Rowling - returns list of books where author is J.K.Rowling (more than one filter is possible)
- book add - starts adding book with folowing info.

All additional information will be collected through questions. Don't worry, you won't have to think anything by yourself.
