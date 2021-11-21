using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VismaBookLibary.Models.DTO
{
    public class BorrowDTO
    {
        public string Weeks { get; set; }
        public string CustomerId { get; set; }
        public string ISBN { get; set; }
    }
}
