using SarasaviLibrary.Data;

namespace SarasaviLibrary.Forms
{
    public class ReturnBookControl : UserControl
    {
        private DashboardForm _dashboard;
        private DataGridView dgvActive = null!;
        private Label lblResult = null!;

        public ReturnBookControl(DashboardForm dashboard)
        {
            _dashboard = dashboard;
            InitializeComponent();
            LoadActiveBorrowings();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;

            Label lblTitle = new Label
            {
                Text = "📥  Return Book",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 78, 121),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            Label lblInstruction = new Label
            {
                Text = "Select a borrowing record and click 'Return Selected Book'",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(10, 42)
            };

            dgvActive = new DataGridView
            {
                Location = new Point(10, 70),
                Size = new Size(870, 300),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 8.5f)
            };
            dgvActive.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 126, 34);
            dgvActive.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvActive.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);

            Button btnReturn = new Button
            {
                Text = "📥  Return Selected Book",
                Location = new Point(10, 385),
                Size = new Size(220, 42),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnReturn.Click += BtnReturn_Click;

            Button btnRefresh = new Button
            {
                Text = "↺ Refresh",
                Location = new Point(240, 385),
                Size = new Size(100, 42),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.Click += (s, e) => LoadActiveBorrowings();

            lblResult = new Label
            {
                Location = new Point(10, 435),
                Size = new Size(870, 30),
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold)
            };

            this.Controls.AddRange(new Control[] { lblTitle, lblInstruction, dgvActive, btnReturn, btnRefresh, lblResult });
        }

        private void LoadActiveBorrowings()
        {
            var borrows = DataManager.GetActiveBorrowings();
            dgvActive.DataSource = null;
            var display = borrows.Select(b => new
            {
                BorrowID = b.BorrowID,
                Book = b.BookTitle,
                Member = b.MemberName,
                BorrowDate = b.BorrowDate.ToString("dd/MM/yyyy"),
                DueDate = b.DueDate.ToString("dd/MM/yyyy"),
                Status = b.Status,
                DaysLeft = b.IsReturned ? "—" : (b.DueDate - DateTime.Now).TotalDays > 0
                    ? $"{(int)(b.DueDate - DateTime.Now).TotalDays} days"
                    : $"⚠ {(int)(DateTime.Now - b.DueDate).TotalDays} days overdue"
            }).ToList();
            dgvActive.DataSource = display;

            // Color overdue rows red
            foreach (DataGridViewRow row in dgvActive.Rows)
            {
                if (row.Cells["Status"]?.Value?.ToString() == "Overdue")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
            }
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            if (dgvActive.SelectedRows.Count == 0)
            {
                lblResult.Text = "⚠ Please select a borrowing record first.";
                lblResult.ForeColor = Color.FromArgb(230, 126, 34);
                return;
            }

            int borrowId = (int)dgvActive.SelectedRows[0].Cells["BorrowID"].Value;
            string bookName = dgvActive.SelectedRows[0].Cells["Book"].Value?.ToString() ?? "";

            var confirm = MessageBox.Show(
                $"Return '{bookName}'?",
                "Confirm Return",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            var (success, message, fine) = DataManager.ReturnBook(borrowId);
            lblResult.Text = success ? $"✔ {message}" : $"✘ {message}";
            lblResult.ForeColor = success ? Color.FromArgb(39, 174, 96) : Color.FromArgb(192, 57, 43);

            if (fine > 0)
                MessageBox.Show($"Fine Collected: Rs. {fine:F2}", "Fine Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (success) { LoadActiveBorrowings(); _dashboard.RefreshDashboard(); }
        }
    }
}
