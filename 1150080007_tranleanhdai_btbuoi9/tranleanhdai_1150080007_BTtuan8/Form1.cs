using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace tranleanhdai_1150080007_BTtuan8
{
    public partial class Form1 : Form
    {
        // ====== KẾT NỐI SQL (Windows Authentication) ======
        // Nếu muốn dùng sa/123: thay bằng
        // @"Data Source=DESKTOP-NG5TJ8L\MSSQLSERVER01;Initial Catalog=QuanLyBanSach;User ID=sa;Password=123;Encrypt=False;TrustServerCertificate=True"
        private readonly string _connStr =
            @"Data Source=DESKTOP-NG5TJ8L\MSSQLSERVER01;Initial Catalog=QuanLyBanSach;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";

        private SqlConnection _conn;
        private SqlDataAdapter _adapter;
        private DataSet _ds;
        private int _rowIndex = -1;

        // ====== UI controls ======
        private Label lblTitle, lblMa, lblTen, lblDiaChi;
        private TextBox txtMa, txtTen, txtDiaChi;
        private DataGridView dgv;
        private Button btnThem, btnSua, btnXoa, btnLamMoi;

        public Form1()
        {
            InitializeComponent();
        }

        // ==================== FORM LOAD ====================
        private void Form1_Load(object sender, EventArgs e)
        {
            BuildUI();
            LoadData();
            ClearForm();
        }

        // ==================== DATA ====================
        private void EnsureConn()
        {
            if (_conn == null) _conn = new SqlConnection(_connStr);
            if (_conn.State == ConnectionState.Closed) _conn.Open();
        }
        private void CloseConn()
        {
            if (_conn != null && _conn.State == ConnectionState.Open) _conn.Close();
        }

        private void LoadData()
        {
            try
            {
                EnsureConn();
                // Quan trọng: AddWithKey để SqlCommandBuilder tạo UPDATE/DELETE dựa trên PK
                string sql = "SELECT MaXB, TenXB, DiaChi FROM dbo.NhaXuatBan";
                _adapter = new SqlDataAdapter(sql, _conn);
                _adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                // Tự động sinh Insert/Update/Delete
                _ = new SqlCommandBuilder(_adapter);

                _ds = new DataSet();
                _adapter.Fill(_ds, "NhaXuatBan");

                dgv.DataSource = _ds.Tables["NhaXuatBan"];
                dgv.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
            finally
            {
                CloseConn();
            }
        }

        private void ClearForm()
        {
            txtMa.Clear();
            txtTen.Clear();
            txtDiaChi.Clear();
            txtMa.Focus();
            _rowIndex = -1;
        }

        // ==================== EVENTS ====================
        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            _rowIndex = e.RowIndex;
            if (_rowIndex < 0 || _rowIndex >= _ds.Tables["NhaXuatBan"].Rows.Count) return;

            DataRow r = _ds.Tables["NhaXuatBan"].Rows[_rowIndex];
            txtMa.Text = r["MaXB"].ToString();
            txtTen.Text = r["TenXB"].ToString();
            txtDiaChi.Text = r["DiaChi"].ToString();
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMa.Text))
                {
                    MessageBox.Show("Mã XB không được để trống."); return;
                }

                var t = _ds.Tables["NhaXuatBan"];
                // Kiểm tra trùng khóa chính
                if (t.Rows.Find(txtMa.Text.Trim()) != null)
                {
                    MessageBox.Show("Mã XB đã tồn tại."); return;
                }

                DataRow row = t.NewRow();
                row["MaXB"] = txtMa.Text.Trim();
                row["TenXB"] = txtTen.Text.Trim();
                row["DiaChi"] = txtDiaChi.Text.Trim();
                t.Rows.Add(row);

                EnsureConn();
                int n = _adapter.Update(t);
                CloseConn();

                if (n > 0) { MessageBox.Show("✅ Thêm thành công."); LoadData(); ClearForm(); }
                else MessageBox.Show("❌ Thêm thất bại.");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (_rowIndex < 0) { MessageBox.Show("Chọn dòng cần sửa."); return; }

                var t = _ds.Tables["NhaXuatBan"];
                DataRow row = t.Rows[_rowIndex];

                row["TenXB"] = txtTen.Text.Trim();
                row["DiaChi"] = txtDiaChi.Text.Trim();

                EnsureConn();
                int n = _adapter.Update(t);
                CloseConn();

                if (n > 0) { MessageBox.Show("✅ Cập nhật thành công."); LoadData(); ClearForm(); }
                else MessageBox.Show("❌ Cập nhật thất bại.");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (_rowIndex < 0) { MessageBox.Show("Chọn dòng cần xóa."); return; }

                if (MessageBox.Show("Bạn chắc chắn muốn xóa?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

                var t = _ds.Tables["NhaXuatBan"];
                t.Rows[_rowIndex].Delete();

                EnsureConn();
                int n = _adapter.Update(t);
                CloseConn();

                if (n > 0) { MessageBox.Show("🗑️ Xóa thành công."); LoadData(); ClearForm(); }
                else MessageBox.Show("❌ Xóa thất bại.");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();
            ClearForm();
        }

        // ==================== UI ====================
        private void BuildUI()
        {
            // Form
            Text = "Quản lý Nhà Xuất Bản - QuanLyBanSach";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 560);

            // Labels
            lblTitle = new Label
            {
                Text = "QUẢN LÝ NHÀ XUẤT BẢN",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Location = new Point(280, 15)
            };

            lblMa = new Label { Text = "Mã XB:", Location = new Point(30, 80), AutoSize = true };
            lblTen = new Label { Text = "Tên XB:", Location = new Point(30, 120), AutoSize = true };
            lblDiaChi = new Label { Text = "Địa chỉ:", Location = new Point(30, 160), AutoSize = true };

            // TextBoxes
            txtMa = new TextBox { Location = new Point(110, 76), Width = 220 };
            txtTen = new TextBox { Location = new Point(110, 116), Width = 220 };
            txtDiaChi = new TextBox { Location = new Point(110, 156), Width = 220 };

            // DataGridView
            dgv = new DataGridView
            {
                Location = new Point(360, 76),
                Size = new Size(510, 400),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.CellClick += Dgv_CellClick;

            // Buttons
            btnThem = new Button { Text = "Thêm", Location = new Point(30, 220), Size = new Size(75, 30) };
            btnSua = new Button { Text = "Sửa", Location = new Point(120, 220), Size = new Size(75, 30) };
            btnXoa = new Button { Text = "Xóa", Location = new Point(210, 220), Size = new Size(75, 30) };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(110, 265), Size = new Size(95, 30) };

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLamMoi.Click += BtnLamMoi_Click;

            // Add controls
            Controls.Add(lblTitle);
            Controls.Add(lblMa);
            Controls.Add(lblTen);
            Controls.Add(lblDiaChi);
            Controls.Add(txtMa);
            Controls.Add(txtTen);
            Controls.Add(txtDiaChi);
            Controls.Add(dgv);
            Controls.Add(btnThem);
            Controls.Add(btnSua);
            Controls.Add(btnXoa);
            Controls.Add(btnLamMoi);
        }
    }
}
