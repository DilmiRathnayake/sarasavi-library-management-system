using SarasaviLibrary.Data;

namespace SarasaviLibrary.Forms
{
    public class BorrowingRecordsControl : UserControl
    {
        private DashboardForm _dashboard;
        private DataGridView dgv = null!;
        private Label lblCount = null!;

        public BorrowingRecordsControl(DashboardForm dashboard)
        {
            _dashboard = dashboard;
            InitializeComponent();
            LoadAll();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;

            Label lblTitle = new Label
            {
                Text = "📋  Borrowing Records (All History)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 78, 121),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            Button btnAll = CreateFilterButton("All", Color.FromArgb(31, 78, 121), new Point(10, 45));
            Button btnActive = CreateFilterButton("Active Only", Color.FromArgb(230, 126, 34), new Point(90, 45));
            Button btnReturned = CreateFilterButton("Returned", Color.FromArgb(39, 174, 96), new Point(200, 45));

            btnAll.Click += (s, e) => LoadAll();
            btnActive.Click += (s, e) => LoadActive();
            btnReturned.Click += (s, e) => LoadReturned();

            lblCount = new Label
            {
                Text = "Total records: 0",
                AutoSize = true,
                Location = new Point(330, 53),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.Gray
            };

            dgv = new DataGridView
            {
                Location = new Point(10, 80),
                Size = new Size(875, 340),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 8.5f)
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);

            this.Controls.AddRange(new Control[] { lblTitle, btnAll, btnActive, btnReturned, lblCount, dgv });
        }

        private Button CreateFilterButton(string text, Color color, Point loc) =>
            new Button
            {
                Text = text, Location = loc, Size = new Size(100, 28),
                BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

        private void LoadAll()
        {
            var records = DataManager.GetAllBorrowings();
            BindGrid(records.Select(b => new
            {
                ID = b.BorrowID,
                Book = b.BookTitle,
                Member = b.MemberName,
                BorrowDate = b.BorrowDate.ToString("dd/MM/yyyy"),
                DueDate = b.DueDate.ToString("dd/MM/yyyy"),
                ReturnDate = b.ReturnDate.HasValue ? b.ReturnDate.Value.ToString("dd/MM/yyyy") : "—",
                Status = b.Status,
                Fine = b.FineAmount > 0 ? $"Rs. {b.FineAmount:F2}" : "—"
            }).ToList<object>(), records.Count);
        }

        private void LoadActive()
        {
            var records = DataManager.GetActiveBorrowings();
            BindGrid(records.Select(b => new
            {
                ID = b.BorrowID,
                Book = b.BookTitle,
                Member = b.MemberName,
                BorrowDate = b.BorrowDate.ToString("dd/MM/yyyy"),
                DueDate = b.DueDate.ToString("dd/MM/yyyy"),
                ReturnDate = "—",
                Status = b.Status,
                Fine = "—"
            }).ToList<object>(), records.Count);
        }

        private void LoadReturned()
        {
            var records = DataManager.GetAllBorrowings().Where(b => b.IsReturned).ToList();
            BindGrid(records.Select(b => new
            {
                ID = b.BorrowID,
                Book = b.BookTitle,
                Member = b.MemberName,
                BorrowDate = b.BorrowDate.ToString("dd/MM/yyyy"),
                DueDate = b.DueDate.ToString("dd/MM/yyyy"),
                ReturnDate = b.ReturnDate.HasValue ? b.ReturnDate.Value.ToString("dd/MM/yyyy") : "—",
                Status = b.Status,
                Fine = b.FineAmount > 0 ? $"Rs. {b.FineAmount:F2}" : "None"
            }).ToList<object>(), records.Count);
        }

        private void BindGrid(List<object> data, int count)
        {
            dgv.DataSource = null;
            dgv.DataSource = data;
            lblCount.Text = $"Total records: {count}";
        }
    }
}
