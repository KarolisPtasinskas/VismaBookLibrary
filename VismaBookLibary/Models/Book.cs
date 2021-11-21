namespace VismaBookLibary.Models
{
    public class Book
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public int PublicationYear { get; set; }
        public string ISBN { get; set; }
        public int Quantity { get; set; }
        public bool Available { get; set; } = true;

    }

}
