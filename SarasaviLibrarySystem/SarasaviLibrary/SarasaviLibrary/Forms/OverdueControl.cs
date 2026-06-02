using SarasaviLibrary.Data;

namespace SarasaviLibrary.Forms
{
    public class OverdueControl : UserControl
    {
        private DashboardForm _dashboard;
        private DataGridView dgv = null!;

        public OverdueControl(DashboardForm dashboard)
        {
            _dashboard = dashboard;
            InitializeComponent();
            LoadOverdue();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;

            Label lblTitle = new Label
            {
                Text = "⚠️  Overdue Books",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(192, 57, 43),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            Panel alertBanner = new Panel
            {
                Location = new Point(10, 42),
                Size = new Size(875, 30),
                BackColor = Color.FromArgb(255, 235, 235)
            };
            Label lblAlert = new Label
            {
                Text = "  ⚠  The following books are overdue. Fine: Rs. 5.00 per day after due date.",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(192, 57, 43),
                AutoSize = true,
                Location = new Point(5, 7)
            };
            alertBanner.Controls.Add(lblAlert);

            dgv = new DataGridView
            {
                Location = new Point(10, 82),
                Size = new Size(875, 310),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 8.5f)
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(192, 57, 43);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);

            Button btnRefresh = new Button
            {
                Text = "↺ Refresh",
                Location = new Point(10, 403),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.Click += (s, e) => LoadOverdue();

            this.Controls.AddRange(new Control[] { lblTitle, alertBanner, dgv, btnRefresh });
        }

        private void LoadOverdue()
        {
            var overdue = DataManager.GetOverdueBorrowings();
            dgv.DataSource = null;
            var display = overdue.Select(b => new
            {
                BorrowID = b.BorrowID,
                Book = b.BookTitle,
                Member = b.MemberName,
                DueDate = b.DueDate.ToString("dd/MM/yyyy"),
                DaysOverdue = (int)(DateTime.Now - b.DueDate).TotalDays,
                EstimatedFine = $"Rs. {(int)(DateTime.Now - b.DueDate).TotalDays * DataManager.FINE_PER_DAY:F2}"
            }).ToList();
            dgv.DataSource = display;

            foreach (DataGridViewRow row in dgv.Rows)
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 240);
        }
    }
}
