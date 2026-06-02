using SarasaviLibrary.Models;

namespace SarasaviLibrary.Data
{
    /// <summary>
    /// DataManager acts as an in-memory database for the Library Management System.
    /// All data is stored in static lists during runtime.
    /// </summary>
    public static class DataManager
    {
        // ===== In-Memory Data Storage =====
        private static List<Book> _books = new List<Book>();
        private static List<Member> _members = new List<Member>();
        private static List<Borrowing> _borrowings = new List<Borrowing>();

        private static int _bookIdCounter = 1;
        private static int _memberIdCounter = 1;
        private static int _borrowIdCounter = 1;

        // Fine rate per day in LKR
        public const double FINE_PER_DAY = 5.0;

        // ==================== BOOK OPERATIONS ====================

        /// <summary>Returns all books in the library.</summary>
        public static List<Book> GetAllBooks() => new List<Book>(_books);

        /// <summary>Searches books by title, author, or ISBN.</summary>
        public static List<Book> SearchBooks(string keyword)
        {
            keyword = keyword.ToLower();
            return _books.Where(b =>
                b.Title.ToLower().Contains(keyword) ||
                b.Author.ToLower().Contains(keyword) ||
                b.ISBN.ToLower().Contains(keyword) ||
                b.Category.ToLower().Contains(keyword)
            ).ToList();
        }

        /// <summary>Adds a new book to the library.</summary>
        public static bool AddBook(Book book)
        {
            // Check for duplicate ISBN
            if (_books.Any(b => b.ISBN == book.ISBN))
                return false;

            book.BookID = _bookIdCounter++;
            book.AvailableQuantity = book.Quantity;
            _books.Add(book);
            return true;
        }

        /// <summary>Updates an existing book's details.</summary>
        public static bool UpdateBook(Book updatedBook)
        {
            var existing = _books.FirstOrDefault(b => b.BookID == updatedBook.BookID);
            if (existing == null) return false;

            int borrowed = existing.Quantity - existing.AvailableQuantity;
            existing.Title = updatedBook.Title;
            existing.Author = updatedBook.Author;
            existing.ISBN = updatedBook.ISBN;
            existing.Category = updatedBook.Category;
            existing.Publisher = updatedBook.Publisher;
            existing.PublishedYear = updatedBook.PublishedYear;
            existing.Quantity = updatedBook.Quantity;
            existing.AvailableQuantity = updatedBook.Quantity - borrowed;
            return true;
        }

        /// <summary>Deletes a book by ID (only if not currently borrowed).</summary>
        public static bool DeleteBook(int bookId)
        {
            var book = _books.FirstOrDefault(b => b.BookID == bookId);
            if (book == null) return false;
            if (book.AvailableQuantity < book.Quantity) return false; // Has active borrows
            _books.Remove(book);
            return true;
        }

        public static Book? GetBookById(int id) => _books.FirstOrDefault(b => b.BookID == id);

        // ==================== MEMBER OPERATIONS ====================

        /// <summary>Returns all library members.</summary>
        public static List<Member> GetAllMembers() => new List<Member>(_members);

        /// <summary>Searches members by name, email, or phone.</summary>
        public static List<Member> SearchMembers(string keyword)
        {
            keyword = keyword.ToLower();
            return _members.Where(m =>
                m.FirstName.ToLower().Contains(keyword) ||
                m.LastName.ToLower().Contains(keyword) ||
                m.Email.ToLower().Contains(keyword) ||
                m.Phone.Contains(keyword)
            ).ToList();
        }

        /// <summary>Adds a new member to the library.</summary>
        public static bool AddMember(Member member)
        {
            if (_members.Any(m => m.Email == member.Email))
                return false;

            member.MemberID = _memberIdCounter++;
            member.MembershipDate = DateTime.Now;
            _members.Add(member);
            return true;
        }

        /// <summary>Updates an existing member's details.</summary>
        public static bool UpdateMember(Member updated)
        {
            var existing = _members.FirstOrDefault(m => m.MemberID == updated.MemberID);
            if (existing == null) return false;

            existing.FirstName = updated.FirstName;
            existing.LastName = updated.LastName;
            existing.Email = updated.Email;
            existing.Phone = updated.Phone;
            existing.Address = updated.Address;
            existing.IsActive = updated.IsActive;
            return true;
        }

        /// <summary>Deletes a member (only if no active borrowings).</summary>
        public static bool DeleteMember(int memberId)
        {
            var member = _members.FirstOrDefault(m => m.MemberID == memberId);
            if (member == null) return false;
            if (_borrowings.Any(b => b.MemberID == memberId && !b.IsReturned)) return false;
            _members.Remove(member);
            return true;
        }

        public static Member? GetMemberById(int id) => _members.FirstOrDefault(m => m.MemberID == id);

        // ==================== BORROWING OPERATIONS ====================

        /// <summary>Returns all borrowing records.</summary>
        public static List<Borrowing> GetAllBorrowings() => new List<Borrowing>(_borrowings);

        /// <summary>Returns only currently active (not returned) borrowings.</summary>
        public static List<Borrowing> GetActiveBorrowings() =>
            _borrowings.Where(b => !b.IsReturned).ToList();

        /// <summary>Returns overdue borrowings.</summary>
        public static List<Borrowing> GetOverdueBorrowings() =>
            _borrowings.Where(b => !b.IsReturned && DateTime.Now > b.DueDate).ToList();

        /// <summary>Issues a book to a member.</summary>
        public static (bool Success, string Message) IssueBook(int bookId, int memberId, int loanDays = 14)
        {
            var book = GetBookById(bookId);
            if (book == null) return (false, "Book not found.");
            if (book.AvailableQuantity <= 0) return (false, "No copies available.");

            var member = GetMemberById(memberId);
            if (member == null) return (false, "Member not found.");
            if (!member.IsActive) return (false, "Member account is inactive.");

            // Check if member already has this book
            if (_borrowings.Any(b => b.BookID == bookId && b.MemberID == memberId && !b.IsReturned))
                return (false, "Member already has this book.");

            // Check member hasn't exceeded borrowing limit (max 3 books)
            int activeCount = _borrowings.Count(b => b.MemberID == memberId && !b.IsReturned);
            if (activeCount >= 3) return (false, "Member has reached the borrowing limit (3 books).");

            book.AvailableQuantity--;

            var borrowing = new Borrowing
            {
                BorrowID = _borrowIdCounter++,
                BookID = bookId,
                MemberID = memberId,
                BookTitle = book.Title,
                MemberName = member.FullName,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(loanDays),
                IsReturned = false
            };

            _borrowings.Add(borrowing);
            return (true, $"Book issued successfully! Due date: {borrowing.DueDate:dd/MM/yyyy}");
        }

        /// <summary>Returns a borrowed book, calculates fine if overdue.</summary>
        public static (bool Success, string Message, double Fine) ReturnBook(int borrowId)
        {
            var borrowing = _borrowings.FirstOrDefault(b => b.BorrowID == borrowId);
            if (borrowing == null) return (false, "Borrowing record not found.", 0);
            if (borrowing.IsReturned) return (false, "Book already returned.", 0);

            var book = GetBookById(borrowing.BookID);
            if (book != null) book.AvailableQuantity++;

            borrowing.ReturnDate = DateTime.Now;
            borrowing.IsReturned = true;

            double fine = 0;
            if (DateTime.Now > borrowing.DueDate)
            {
                int daysLate = (int)(DateTime.Now - borrowing.DueDate).TotalDays;
                fine = daysLate * FINE_PER_DAY;
                borrowing.FineAmount = fine;
            }

            string msg = fine > 0
                ? $"Book returned. Fine: Rs. {fine:F2} ({(int)(DateTime.Now - borrowing.DueDate).TotalDays} days overdue)"
                : "Book returned successfully. No fine.";

            return (true, msg, fine);
        }

        // ==================== STATISTICS ====================

        public static int TotalBooks => _books.Count;
        public static int TotalMembers => _members.Count;
        public static int ActiveBorrowings => _borrowings.Count(b => !b.IsReturned);
        public static int OverdueBorrowings => GetOverdueBorrowings().Count;

        // ==================== SEED DATA ====================

        /// <summary>Loads sample data for demonstration purposes.</summary>
        public static void LoadSampleData()
        {
            // Sample Books
            AddBook(new Book { Title = "Clean Code", Author = "Robert C. Martin", ISBN = "978-0132350884", Category = "Programming", Quantity = 3, Publisher = "Prentice Hall", PublishedYear = 2008 });
            AddBook(new Book { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", ISBN = "978-0743273565", Category = "Fiction", Quantity = 5, Publisher = "Scribner", PublishedYear = 1925 });
            AddBook(new Book { Title = "Design Patterns", Author = "Gang of Four", ISBN = "978-0201633610", Category = "Programming", Quantity = 2, Publisher = "Addison-Wesley", PublishedYear = 1994 });
            AddBook(new Book { Title = "Sapiens", Author = "Yuval Noah Harari", ISBN = "978-0062316097", Category = "History", Quantity = 4, Publisher = "Harper", PublishedYear = 2015 });
            AddBook(new Book { Title = "Introduction to Algorithms", Author = "Cormen et al.", ISBN = "978-0262033848", Category = "Computer Science", Quantity = 2, Publisher = "MIT Press", PublishedYear = 2009 });

            // Sample Members
            AddMember(new Member { FirstName = "Kamal", LastName = "Perera", Email = "kamal@gmail.com", Phone = "0711234567", Address = "Colombo 03" });
            AddMember(new Member { FirstName = "Nimali", LastName = "Silva", Email = "nimali@gmail.com", Phone = "0759876543", Address = "Kandy" });
            AddMember(new Member { FirstName = "Ruwan", LastName = "Fernando", Email = "ruwan@gmail.com", Phone = "0774561230", Address = "Galle" });
        }
    }
}
