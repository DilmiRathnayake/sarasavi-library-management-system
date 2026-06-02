using SarasaviLibrary.Data;

namespace SarasaviLibrary.Forms
{
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
            DataManager.LoadSampleData();
            RefreshDashboard();
        }

        private void InitializeComponent()
        {
            this.Text = "Sarasavi Library Management System";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 248, 255);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 9f);

            // ===== HEADER PANEL =====
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(31, 78, 121)
            };

            Label lblTitle = new Label
            {
                Text = "📚  SARASAVI LIBRARY MANAGEMENT SYSTEM",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 22)
            };

            Label lblSubtitle = new Label
            {
                Text = "Institute of Management & Business Studies",
                ForeColor = Color.FromArgb(173, 216, 230),
                Font = new Font("Segoe UI", 9f),
                AutoSize = true,
                Location = new Point(22, 54)
            };

            headerPanel.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });

            // ===== STATISTICS PANEL =====
            Panel statsPanel = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(1100, 120),
                BackColor = Color.FromArgb(235, 242, 255)
            };

            // 4 stat cards
            lblStatBooks = CreateStatCard("Total Books", "0", Color.FromArgb(41, 128, 185), new Point(20, 15));
            lblStatMembers = CreateStatCard("Total Members", "0", Color.FromArgb(39, 174, 96), new Point(290, 15));
            lblStatBorrowed = CreateStatCard("Active Borrows", "0", Color.FromArgb(230, 126, 34), new Point(560, 15));
            lblStatOverdue = CreateStatCard("Overdue", "0", Color.FromArgb(192, 57, 43), new Point(830, 15));

            statsPanel.Controls.AddRange(new Control[] { lblStatBooks, lblStatMembers, lblStatBorrowed, lblStatOverdue });

            // ===== NAVIGATION BUTTONS =====
            Panel navPanel = new Panel
            {
                Location = new Point(0, 200),
                Size = new Size(200, 450),
                BackColor = Color.FromArgb(31, 78, 121)
            };

            btnDashboard = CreateNavButton("🏠  Dashboard", new Point(0, 10), true);
            btnBooks = CreateNavButton("📖  Manage Books", new Point(0, 60), false);
            btnMembers = CreateNavButton("👥  Manage Members", new Point(0, 110), false);
            btnBorrow = CreateNavButton("📤  Issue Book", new Point(0, 160), false);
            btnReturn = CreateNavButton("📥  Return Book", new Point(0, 210), false);
            btnBorrowings = CreateNavButton("📋  Borrowing Records", new Point(0, 260), false);
            btnOverdue = CreateNavButton("⚠️  Overdue Books", new Point(0, 310), false);

            btnDashboard.Click += (s, e) => ShowDashboard();
            btnBooks.Click += (s, e) => OpenBooks();
            btnMembers.Click += (s, e) => OpenMembers();
            btnBorrow.Click += (s, e) => OpenIssueBook();
            btnReturn.Click += (s, e) => OpenReturnBook();
            btnBorrowings.Click += (s, e) => OpenBorrowings();
            btnOverdue.Click += (s, e) => OpenOverdue();

            navPanel.Controls.AddRange(new Control[] { btnDashboard, btnBooks, btnMembers, btnBorrow, btnReturn, btnBorrowings, btnOverdue });

            // ===== MAIN CONTENT PANEL =====
            mainContentPanel = new Panel
            {
                Location = new Point(200, 200),
                Size = new Size(900, 450),
                BackColor = Color.White
            };

            // Welcome label on dashboard
            lblWelcome = new Label
            {
                Text = "Welcome to Sarasavi Library Management System\r\n\r\n" +
                       "Use the navigation menu on the left to manage:\r\n\r\n" +
                       "  📖  Books — Add, edit, delete, and search books\r\n" +
                       "  👥  Members — Register and manage library members\r\n" +
                       "  📤  Issue Book — Issue books to registered members\r\n" +
                       "  📥  Return Book — Process book returns and calculate fines\r\n" +
                       "  📋  Borrowing Records — View all borrowing history\r\n" +
                       "  ⚠️  Overdue Books — View and manage overdue books",
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(50, 50, 80),
                Location = new Point(40, 50),
                Size = new Size(820, 380),
                AutoSize = false
            };

            mainContentPanel.Controls.Add(lblWelcome);

            // ===== STATUS BAR =====
            Panel statusBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 28,
                BackColor = Color.FromArgb(31, 78, 121)
            };

            lblStatus = new Label
            {
                Text = $"  System Ready  |  {DateTime.Now:dddd, dd MMMM yyyy  HH:mm}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f),
                AutoSize = true,
                Location = new Point(5, 6)
            };

            statusBar.Controls.Add(lblStatus);

            this.Controls.AddRange(new Control[] { headerPanel, statsPanel, navPanel, mainContentPanel, statusBar });
        }

        // ===== UI Controls =====
        private Panel mainContentPanel = null!;
        private Label lblWelcome = null!;
        private Label lblStatus = null!;
        private Panel lblStatBooks = null!, lblStatMembers = null!, lblStatBorrowed = null!, lblStatOverdue = null!;
        private Button btnDashboard = null!, btnBooks = null!, btnMembers = null!;
        private Button btnBorrow = null!, btnReturn = null!, btnBorrowings = null!, btnOverdue = null!;

        private Panel CreateStatCard(string title, string value, Color color, Point location)
        {
            Panel card = new Panel
            {
                Size = new Size(240, 88),
                Location = location,
                BackColor = color
            };
            Label lVal = new Label
            {
                Name = "value",
                Text = value,
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(18, 12)
            };
            Label lTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(220, 220, 220),
                AutoSize = true,
                Location = new Point(18, 58)
            };
            card.Controls.AddRange(new Control[] { lVal, lTitle });
            return card;
        }

        private Button CreateNavButton(string text, Point location, bool active)
        {
            return new Button
            {
                Text = text,
                Size = new Size(200, 48),
                Location = location,
                FlatStyle = FlatStyle.Flat,
                BackColor = active ? Color.FromArgb(52, 110, 160) : Color.FromArgb(31, 78, 121),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        public void RefreshDashboard()
        {
            UpdateStatCard(lblStatBooks, DataManager.TotalBooks.ToString());
            UpdateStatCard(lblStatMembers, DataManager.TotalMembers.ToString());
            UpdateStatCard(lblStatBorrowed, DataManager.ActiveBorrowings.ToString());
            UpdateStatCard(lblStatOverdue, DataManager.OverdueBorrowings.ToString());
            if (lblStatus != null)
                lblStatus.Text = $"  System Ready  |  {DateTime.Now:dddd, dd MMMM yyyy  HH:mm}";
        }

        private void UpdateStatCard(Panel card, string value)
        {
            var lbl = card.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "value");
            if (lbl != null) lbl.Text = value;
        }

        private void ShowDashboard()
        {
            mainContentPanel.Controls.Clear();
            mainContentPanel.Controls.Add(lblWelcome);
            RefreshDashboard();
        }

        private void OpenBooks()
        {
            mainContentPanel.Controls.Clear();
            var ctrl = new BookManagementControl(this);
            ctrl.Dock = DockStyle.Fill;
            mainContentPanel.Controls.Add(ctrl);
        }

        private void OpenMembers()
        {
            mainContentPanel.Controls.Clear();
            var ctrl = new MemberManagementControl(this);
            ctrl.Dock = DockStyle.Fill;
            mainContentPanel.Controls.Add(ctrl);
        }

        private void OpenIssueBook()
        {
            mainContentPanel.Controls.Clear();
            var ctrl = new IssueBookControl(this);
            ctrl.Dock = DockStyle.Fill;
            mainContentPanel.Controls.Add(ctrl);
        }

        private void OpenReturnBook()
        {
            mainContentPanel.Controls.Clear();
            var ctrl = new ReturnBookControl(this);
            ctrl.Dock = DockStyle.Fill;
            mainContentPanel.Controls.Add(ctrl);
        }

        private void OpenBorrowings()
        {
            mainContentPanel.Controls.Clear();
            var ctrl = new BorrowingRecordsControl(this);
            ctrl.Dock = DockStyle.Fill;
            mainContentPanel.Controls.Add(ctrl);
        }

        private void OpenOverdue()
        {
            mainContentPanel.Controls.Clear();
            var ctrl = new OverdueControl(this);
            ctrl.Dock = DockStyle.Fill;
            mainContentPanel.Controls.Add(ctrl);
        }
    }
}
