using System.Collections.Generic;

namespace VismaBookLibary.Models
{
    class AppCommands
    {
        public List<string> Objects { get; } = new List<string> { "books" };
        public List<string> BookMainCommands { get; } = new List<string> { "list", "filter", "add", "take", "return", "delete" };
    }
}
