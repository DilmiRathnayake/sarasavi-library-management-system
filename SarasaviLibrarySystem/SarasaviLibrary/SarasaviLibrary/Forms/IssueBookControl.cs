using SarasaviLibrary.Data;

namespace SarasaviLibrary.Forms
{
    public class IssueBookControl : UserControl
    {
        private DashboardForm _dashboard;
        private ComboBox cmbBook = null!, cmbMember = null!;
        private NumericUpDown numDays = null!;
        private Label lblBookInfo = null!, lblMemberInfo = null!, lblResult = null!;

        public IssueBookControl(DashboardForm dashboard)
        {
            _dashboard = dashboard;
            InitializeComponent();
            LoadCombos();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;

            Label lblTitle = new Label
            {
                Text = "📤  Issue Book to Member",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 78, 121),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            // Card panel
            Panel card = new Panel
            {
                Location = new Point(60, 55),
                Size = new Size(460, 330),
                BackColor = Color.FromArgb(245, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            int y = 20;

            Label lbl1 = new Label { Text = "Select Book *", AutoSize = true, Location = new Point(20, y), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            cmbBook = new ComboBox { Location = new Point(20, y + 22), Size = new Size(415, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cmbBook.SelectedIndexChanged += (s, e) => ShowBookInfo();
            y += 60;

            lblBookInfo = new Label { Location = new Point(20, y), Size = new Size(415, 30), Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(39, 174, 96) };
            y += 38;

            Label lbl2 = new Label { Text = "Select Member *", AutoSize = true, Location = new Point(20, y), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            cmbMember = new ComboBox { Location = new Point(20, y + 22), Size = new Size(415, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5f) };
            cmbMember.SelectedIndexChanged += (s, e) => ShowMemberInfo();
            y += 60;

            lblMemberInfo = new Label { Location = new Point(20, y), Size = new Size(415, 30), Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(52, 152, 219) };
            y += 38;

            Label lbl3 = new Label { Text = "Loan Duration (Days) *", AutoSize = true, Location = new Point(20, y), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            numDays = new NumericUpDown { Location = new Point(20, y + 22), Size = new Size(100, 28), Minimum = 1, Maximum = 60, Value = 14, Font = new Font("Segoe UI", 10f) };
            y += 60;

            Button btnIssue = new Button
            {
                Text = "📤  ISSUE BOOK",
                Location = new Point(20, y),
                Size = new Size(415, 42),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnIssue.Click += BtnIssue_Click;

            card.Controls.AddRange(new Control[] { lbl1, cmbBook, lblBookInfo, lbl2, cmbMember, lblMemberInfo, lbl3, numDays, btnIssue });

            lblResult = new Label
            {
                Location = new Point(60, 400),
                Size = new Size(460, 40),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.Controls.AddRange(new Control[] { lblTitle, card, lblResult });
        }

        private void LoadCombos()
        {
            cmbBook.Items.Clear();
            foreach (var b in DataManager.GetAllBooks().Where(b => b.AvailableQuantity > 0))
                cmbBook.Items.Add(new ComboItem(b.BookID, $"[{b.BookID}] {b.Title} by {b.Author} (Avail: {b.AvailableQuantity})"));

            cmbMember.Items.Clear();
            foreach (var m in DataManager.GetAllMembers().Where(m => m.IsActive))
                cmbMember.Items.Add(new ComboItem(m.MemberID, $"[{m.MemberID}] {m.FullName} — {m.Email}"));
        }

        private void ShowBookInfo()
        {
            if (cmbBook.SelectedItem is ComboItem item)
            {
                var book = DataManager.GetBookById(item.Id);
                if (book != null)
                    lblBookInfo.Text = $"Category: {book.Category}  |  Available: {book.AvailableQuantity}/{book.Quantity}";
            }
        }

        private void ShowMemberInfo()
        {
            if (cmbMember.SelectedItem is ComboItem item)
            {
                var member = DataManager.GetMemberById(item.Id);
                if (member != null)
                {
                    int active = DataManager.GetActiveBorrowings().Count(b => b.MemberID == item.Id);
                    lblMemberInfo.Text = $"Phone: {member.Phone}  |  Currently borrowed: {active}/3 books";
                }
            }
        }

        private void BtnIssue_Click(object? sender, EventArgs e)
        {
            if (cmbBook.SelectedItem == null || cmbMember.SelectedItem == null)
            {
                lblResult.Text = "⚠ Please select both a book and a member.";
                lblResult.ForeColor = Color.FromArgb(230, 126, 34);
                return;
            }

            int bookId = ((ComboItem)cmbBook.SelectedItem).Id;
            int memberId = ((ComboItem)cmbMember.SelectedItem).Id;
            int days = (int)numDays.Value;

            var (success, message) = DataManager.IssueBook(bookId, memberId, days);
            lblResult.Text = success ? $"✔ {message}" : $"✘ {message}";
            lblResult.ForeColor = success ? Color.FromArgb(39, 174, 96) : Color.FromArgb(192, 57, 43);

            if (success)
            {
                cmbBook.SelectedIndex = -1;
                cmbMember.SelectedIndex = -1;
                lblBookInfo.Text = "";
                lblMemberInfo.Text = "";
                LoadCombos();
                _dashboard.RefreshDashboard();
            }
        }
    }

    // Helper class for ComboBox items with an integer ID
    public class ComboItem
    {
        public int Id { get; }
        private string _display;
        public ComboItem(int id, string display) { Id = id; _display = display; }
        public override string ToString() => _display;
    }
}
