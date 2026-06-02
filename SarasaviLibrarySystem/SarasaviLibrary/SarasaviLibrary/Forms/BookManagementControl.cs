using SarasaviLibrary.Data;
using SarasaviLibrary.Models;

namespace SarasaviLibrary.Forms
{
    public class BookManagementControl : UserControl
    {
        private DashboardForm _dashboard;
        private DataGridView dgvBooks = null!;
        private TextBox txtSearch = null!, txtTitle = null!, txtAuthor = null!,
                        txtISBN = null!, txtPublisher = null!;
        private ComboBox cmbCategory = null!;
        private NumericUpDown numQuantity = null!, numYear = null!;
        private Button btnAdd = null!, btnUpdate = null!, btnDelete = null!, btnClear = null!;
        private int selectedBookId = -1;

        public BookManagementControl(DashboardForm dashboard)
        {
            _dashboard = dashboard;
            InitializeComponent();
            LoadBooks();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;

            // Title
            Label lblTitle = new Label
            {
                Text = "📖  Book Management",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 78, 121),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            // Search
            Label lblSearch = new Label { Text = "Search:", AutoSize = true, Location = new Point(10, 48) };
            txtSearch = new TextBox { Location = new Point(60, 45), Size = new Size(260, 25) };
            Button btnSearch = new Button
            {
                Text = "🔍 Search",
                Location = new Point(328, 43),
                Size = new Size(90, 27),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            Button btnRefresh = new Button
            {
                Text = "↺ All",
                Location = new Point(425, 43),
                Size = new Size(65, 27),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnSearch.Click += (s, e) => SearchBooks();
            btnRefresh.Click += (s, e) => { txtSearch.Text = ""; LoadBooks(); };
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) SearchBooks(); };

            // DataGridView
            dgvBooks = new DataGridView
            {
                Location = new Point(10, 80),
                Size = new Size(570, 320),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 8.5f)
            };
            dgvBooks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 78, 121);
            dgvBooks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBooks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvBooks.SelectionChanged += DgvBooks_SelectionChanged;

            // ===== RIGHT PANEL - FORM =====
            Panel formPanel = new Panel
            {
                Location = new Point(590, 75),
                Size = new Size(300, 335),
                BackColor = Color.FromArgb(245, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblForm = new Label
            {
                Text = "Book Details",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 78, 121),
                AutoSize = true,
                Location = new Point(10, 8)
            };

            // Form fields
            int y = 35;
            txtTitle = CreateFormField(formPanel, "Title *", ref y);
            txtAuthor = CreateFormField(formPanel, "Author *", ref y);
            txtISBN = CreateFormField(formPanel, "ISBN *", ref y);

            Label lblCat = new Label { Text = "Category *", AutoSize = true, Location = new Point(10, y) };
            cmbCategory = new ComboBox
            {
                Location = new Point(10, y + 18),
                Size = new Size(275, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.AddRange(new object[] { "Programming", "Fiction", "Non-Fiction", "History", "Science", "Computer Science", "Biography", "Children", "Reference", "Other" });
            y += 48;

            txtPublisher = CreateFormField(formPanel, "Publisher", ref y);

            Label lblQty = new Label { Text = "Quantity *", AutoSize = true, Location = new Point(10, y) };
            numQuantity = new NumericUpDown { Location = new Point(10, y + 18), Size = new Size(100, 25), Minimum = 1, Maximum = 100, Value = 1 };

            Label lblYear = new Label { Text = "Year", AutoSize = true, Location = new Point(150, y) };
            numYear = new NumericUpDown { Location = new Point(150, y + 18), Size = new Size(135, 25), Minimum = 1900, Maximum = DateTime.Now.Year, Value = DateTime.Now.Year };
            y += 50;

            formPanel.Controls.AddRange(new Control[] { lblForm, lblCat, cmbCategory, lblQty, numQuantity, lblYear, numYear });

            // Buttons
            btnAdd = CreateActionButton("➕ Add Book", Color.FromArgb(39, 174, 96), new Point(590, 415));
            btnUpdate = CreateActionButton("✏️ Update", Color.FromArgb(52, 152, 219), new Point(730, 415));
            btnDelete = CreateActionButton("🗑️ Delete", Color.FromArgb(192, 57, 43), new Point(730, 355));
            btnClear = CreateActionButton("✖ Clear", Color.FromArgb(149, 165, 166), new Point(590, 355));

            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => ClearForm();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblSearch, txtSearch, btnSearch, btnRefresh,
                dgvBooks, formPanel, btnAdd, btnUpdate, btnDelete, btnClear
            });
        }

        private TextBox CreateFormField(Panel panel, string label, ref int y)
        {
            Label lbl = new Label { Text = label, AutoSize = true, Location = new Point(10, y) };
            TextBox txt = new TextBox { Location = new Point(10, y + 18), Size = new Size(275, 25) };
            panel.Controls.AddRange(new Control[] { lbl, txt });
            y += 48;
            return txt;
        }

        private Button CreateActionButton(string text, Color color, Point loc)
        {
            return new Button
            {
                Text = text,
                Location = loc,
                Size = new Size(130, 35),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void LoadBooks()
        {
            var books = DataManager.GetAllBooks();
            BindGrid(books);
        }

        private void SearchBooks()
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text)) { LoadBooks(); return; }
            BindGrid(DataManager.SearchBooks(txtSearch.Text));
        }

        private void BindGrid(List<Book> books)
        {
            dgvBooks.DataSource = null;
            var display = books.Select(b => new
            {
                ID = b.BookID,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                Category = b.Category,
                Available = $"{b.AvailableQuantity}/{b.Quantity}"
            }).ToList();
            dgvBooks.DataSource = display;
            if (dgvBooks.Columns["ID"] != null) dgvBooks.Columns["ID"].Width = 40;
        }

        private void DgvBooks_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0) return;
            int id = (int)dgvBooks.SelectedRows[0].Cells["ID"].Value;
            var book = DataManager.GetBookById(id);
            if (book == null) return;

            selectedBookId = book.BookID;
            txtTitle.Text = book.Title;
            txtAuthor.Text = book.Author;
            txtISBN.Text = book.ISBN;
            cmbCategory.SelectedItem = book.Category;
            txtPublisher.Text = book.Publisher;
            numQuantity.Value = book.Quantity;
            numYear.Value = book.PublishedYear > 0 ? book.PublishedYear : DateTime.Now.Year;

            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
            btnAdd.Enabled = false;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (!ValidateForm()) return;
            var book = BuildBookFromForm();
            if (DataManager.AddBook(book))
            {
                MessageBox.Show("Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm(); LoadBooks(); _dashboard.RefreshDashboard();
            }
            else MessageBox.Show("A book with this ISBN already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (!ValidateForm() || selectedBookId == -1) return;
            var book = BuildBookFromForm();
            book.BookID = selectedBookId;
            if (DataManager.UpdateBook(book))
            {
                MessageBox.Show("Book updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm(); LoadBooks(); _dashboard.RefreshDashboard();
            }
            else MessageBox.Show("Update failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (selectedBookId == -1) return;
            if (MessageBox.Show("Delete this book?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (DataManager.DeleteBook(selectedBookId))
                {
                    MessageBox.Show("Book deleted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm(); LoadBooks(); _dashboard.RefreshDashboard();
                }
                else MessageBox.Show("Cannot delete: book has active borrowings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Book BuildBookFromForm() => new Book
        {
            Title = txtTitle.Text.Trim(),
            Author = txtAuthor.Text.Trim(),
            ISBN = txtISBN.Text.Trim(),
            Category = cmbCategory.SelectedItem?.ToString() ?? "Other",
            Publisher = txtPublisher.Text.Trim(),
            Quantity = (int)numQuantity.Value,
            PublishedYear = (int)numYear.Value
        };

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtAuthor.Text) ||
                string.IsNullOrWhiteSpace(txtISBN.Text) ||
                cmbCategory.SelectedIndex < 0)
            {
                MessageBox.Show("Please fill all required fields (*).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            txtTitle.Text = txtAuthor.Text = txtISBN.Text = txtPublisher.Text = "";
            cmbCategory.SelectedIndex = -1;
            numQuantity.Value = 1;
            numYear.Value = DateTime.Now.Year;
            selectedBookId = -1;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            dgvBooks.ClearSelection();
        }
    }
}
