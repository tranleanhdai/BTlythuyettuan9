using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Phan4ADO.NET
{
    public partial class frmMatHang : Form
    {
        //Các biến toàn cục
        private SqlConnection con;
        private SqlCommand cmd;
        private SqlDataAdapter dap;
        private DataSet ds;

        public frmMatHang()
        {
            InitializeComponent();
        }

        private void frmMatHang_Load(object sender, EventArgs e)
        {
            //Khoi tạo đối tượng Connection
            con = new SqlConnection();
            con.ConnectionString = Properties.Settings.Default.QLBanHangConnectionString;
            //
            HienThiDL();
        }
        private void HienThiDL()
        {
            //Khoi tao DataSet và DataAdapter
            ds = new DataSet();
            String sql = "Select * from tblMatHang";
            dap = new SqlDataAdapter(sql, con);
            dap.Fill(ds);//gắn dữ liệu vào DataSet
            //Gắn lên datagridView
            dgvMatHang.DataSource = ds.Tables[0];
            dgvMatHang.Refresh();
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            lblTieuDe.Text = "THÊM MẶT HÀNG";
            //Reset các control
            txtMaSP.Text = "";
            txtTenSP.Text = "";
            dtpNgaySX.Value = DateTime.Today;
            dtpNgayHH.Value = DateTime.Today;
            txtDonGia.Text = "";
            txtDonVi.Text = "";
            txtGhiChu.Text = "";
            //Cấm click
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            //
            btnLuu.Visible = true;
            btnHuy.Visible = true;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            //trường hợp thêm mới
            if (btnThem.Enabled == true)
            {
                string sql = "INSERT INTO tblMatHang(MaSP,TenSP,NgaySX)";
                sql += "VALUES(N'"+txtMaSP.Text+"','"+txtTenSP.Text+"','"+ dtpNgaySX.Value.Date +"')";
                //Mở kết nối
                if (con.State != ConnectionState.Open)
                    con.Open();
                //Khoi tạo đối tượng cmd
                cmd = new SqlCommand();
                cmd.CommandText = sql;
                cmd.Connection = con;
                //Thực thi câu lệnh truy vấn
                cmd.ExecuteNonQuery();
                //
                HienThiDL();
            }
        }
    }
}
