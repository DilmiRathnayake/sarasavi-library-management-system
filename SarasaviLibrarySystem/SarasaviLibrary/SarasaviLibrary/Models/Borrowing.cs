namespace SarasaviLibrary.Models
{
    public class Borrowing
    {
        public int BorrowID { get; set; }
        public int BookID { get; set; }
        public int MemberID { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public double FineAmount { get; set; }

        public string Status => IsReturned ? "Returned" : (DateTime.Now > DueDate ? "Overdue" : "Borrowed");

        public override string ToString()
        {
            return $"{BorrowID} | {BookTitle} | {MemberName} | Due: {DueDate:dd/MM/yyyy} | {Status}";
        }
    }
}
