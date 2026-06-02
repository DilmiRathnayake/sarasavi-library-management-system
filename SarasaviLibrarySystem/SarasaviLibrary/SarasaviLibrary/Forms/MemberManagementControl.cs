using SarasaviLibrary.Data;
using SarasaviLibrary.Models;

namespace SarasaviLibrary.Forms
{
    public class MemberManagementControl : UserControl
    {
        private DashboardForm _dashboard;
        private DataGridView dgvMembers = null!;
        private TextBox txtSearch = null!, txtFirstName = null!, txtLastName = null!,
                        txtEmail = null!, txtPhone = null!, txtAddress = null!;
        private CheckBox chkActive = null!;
        private Button btnAdd = null!, btnUpdate = null!, btnDelete = null!;
        private int selectedMemberId = -1;

        public MemberManagementControl(DashboardForm dashboard)
        {
            _dashboard = dashboard;
            InitializeComponent();
            LoadMembers();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;

            Label lblTitle = new Label
            {
                Text = "👥  Member Management",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 78, 121),
                AutoSize = true,
                Location = new Point(10, 10)
            };

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

            btnSearch.Click += (s, e) => SearchMembers();
            btnRefresh.Click += (s, e) => { txtSearch.Text = ""; LoadMembers(); };
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) SearchMembers(); };

            dgvMembers = new DataGridView
            {
                Location = new Point(10, 80),
                Size = new Size(570, 320),
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
            dgvMembers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 78, 121);
            dgvMembers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMembers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvMembers.SelectionChanged += DgvMembers_SelectionChanged;

            // Form panel
            Panel formPanel = new Panel
            {
                Location = new Point(590, 75),
                Size = new Size(300, 355),
                BackColor = Color.FromArgb(245, 248, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblForm = new Label
            {
                Text = "Member Details",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 78, 121),
                AutoSize = true,
                Location = new Point(10, 8)
            };

            int y = 35;
            txtFirstName = CreateFormField(formPanel, "First Name *", ref y);
            txtLastName = CreateFormField(formPanel, "Last Name *", ref y);
            txtEmail = CreateFormField(formPanel, "Email *", ref y);
            txtPhone = CreateFormField(formPanel, "Phone", ref y);
            txtAddress = CreateFormField(formPanel, "Address", ref y);

            chkActive = new CheckBox
            {
                Text = "Active Member",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 9.5f)
            };
            formPanel.Controls.AddRange(new Control[] { lblForm, chkActive });

            btnAdd = CreateActionButton("➕ Add Member", Color.FromArgb(39, 174, 96), new Point(590, 437));
            btnUpdate = CreateActionButton("✏️ Update", Color.FromArgb(52, 152, 219), new Point(730, 437));
            btnDelete = CreateActionButton("🗑️ Delete", Color.FromArgb(192, 57, 43), new Point(730, 395));
            Button btnClear = CreateActionButton("✖ Clear", Color.FromArgb(149, 165, 166), new Point(590, 395));

            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => ClearForm();

            this.Controls.AddRange(new Control[] {
                lblTitle, lblSearch, txtSearch, btnSearch, btnRefresh,
                dgvMembers, formPanel, btnAdd, btnUpdate, btnDelete, btnClear
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

        private Button CreateActionButton(string text, Color color, Point loc) =>
            new Button
            {
                Text = text, Location = loc, Size = new Size(130, 35),
                BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand, FlatAppearance = { BorderSize = 0 }
            };

        private void LoadMembers()
        {
            var members = DataManager.GetAllMembers();
            BindGrid(members);
        }

        private void SearchMembers()
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text)) { LoadMembers(); return; }
            BindGrid(DataManager.SearchMembers(txtSearch.Text));
        }

        private void BindGrid(List<Member> members)
        {
            dgvMembers.DataSource = null;
            var display = members.Select(m => new
            {
                ID = m.MemberID,
                Name = m.FullName,
                Email = m.Email,
                Phone = m.Phone,
                Active = m.IsActive ? "✔ Yes" : "✘ No",
                Since = m.MembershipDate.ToString("dd/MM/yyyy")
            }).ToList();
            dgvMembers.DataSource = display;
        }

        private void DgvMembers_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0) return;
            int id = (int)dgvMembers.SelectedRows[0].Cells["ID"].Value;
            var m = DataManager.GetMemberById(id);
            if (m == null) return;
            selectedMemberId = m.MemberID;
            txtFirstName.Text = m.FirstName;
            txtLastName.Text = m.LastName;
            txtEmail.Text = m.Email;
            txtPhone.Text = m.Phone;
            txtAddress.Text = m.Address;
            chkActive.Checked = m.IsActive;
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
            btnAdd.Enabled = false;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (!ValidateForm()) return;
            var member = BuildMemberFromForm();
            if (DataManager.AddMember(member))
            {
                MessageBox.Show("Member registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm(); LoadMembers(); _dashboard.RefreshDashboard();
            }
            else MessageBox.Show("A member with this email already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (!ValidateForm() || selectedMemberId == -1) return;
            var member = BuildMemberFromForm();
            member.MemberID = selectedMemberId;
            if (DataManager.UpdateMember(member))
            {
                MessageBox.Show("Member updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm(); LoadMembers(); _dashboard.RefreshDashboard();
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (selectedMemberId == -1) return;
            if (MessageBox.Show("Delete this member?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (DataManager.DeleteMember(selectedMemberId))
                { MessageBox.Show("Member deleted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information); ClearForm(); LoadMembers(); _dashboard.RefreshDashboard(); }
                else MessageBox.Show("Cannot delete: member has active borrowings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Member BuildMemberFromForm() => new Member
        {
            FirstName = txtFirstName.Text.Trim(),
            LastName = txtLastName.Text.Trim(),
            Email = txtEmail.Text.Trim(),
            Phone = txtPhone.Text.Trim(),
            Address = txtAddress.Text.Trim(),
            IsActive = chkActive.Checked
        };

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            { MessageBox.Show("Please fill required fields (*).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            return true;
        }

        private void ClearForm()
        {
            txtFirstName.Text = txtLastName.Text = txtEmail.Text = txtPhone.Text = txtAddress.Text = "";
            chkActive.Checked = true;
            selectedMemberId = -1;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            dgvMembers.ClearSelection();
        }
    }
}
