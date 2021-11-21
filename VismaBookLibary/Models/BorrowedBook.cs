using System;

namespace VismaBookLibary.Models
{
    public class BorrowedBook
    {
        public string ISBN { get; set; }
        public string CustomerId { get; set; }
        public DateTime BorrowStart { get; set; } = DateTime.Today;
        public DateTime BorrowEnds { get; set; }
    }
}
