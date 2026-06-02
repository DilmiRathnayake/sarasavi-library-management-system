namespace SarasaviLibrary.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public int PublishedYear { get; set; }

        public override string ToString()
        {
            return $"{BookID} | {Title} | {Author} | {ISBN} | {Category} | Available: {AvailableQuantity}/{Quantity}";
        }
    }
}
