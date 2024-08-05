using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.Entity;


namespace QuanLySach
{
    public partial class Form1 : Form
    {

        private Model1 context;
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            dgvSach.CellContentClick += dataGridView1_CellContentClick;
            btnXoa.Click += button3_Click;
            btnThem.Click += button1_Click;
            btnSua.Click += button2_Click; // Reusing the same handler for simplicity
            txtSearch.TextChanged += textBox1_TextChanged;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            context = new Model1();

            LoadData();
        }

        private void LoadData()
        {
            // Load LoaiSach into ComboBox
            var loaiSachList = context.LoaiSaches.ToList();
            cmbLoaiSach.DataSource = loaiSachList;
            cmbLoaiSach.DisplayMember = "TenLoai";
            cmbLoaiSach.ValueMember = "MaLoai";

            // Load Sach into DataGridView
            var sachList = context.Saches.Include(s => s.LoaiSach).ToList();
            BindGrid(sachList);
        }

        private void BindGrid(List<Sach> sachList)
        {
            dgvSach.Rows.Clear();
            foreach (var sach in sachList)
            {
                int index = dgvSach.Rows.Add();
                dgvSach.Rows[index].Cells["MaSach"].Value = sach.MaSach;
                dgvSach.Rows[index].Cells["TenSach"].Value = sach.TenSach;
                dgvSach.Rows[index].Cells["NamXB"].Value = sach.NamXB;
                dgvSach.Rows[index].Cells["TenLoai"].Value = sach.LoaiSach.TenLoai;
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            var maSach = txtMaSach.Text;
            var sach = context.Saches.FirstOrDefault(s => s.MaSach == maSach);

            if (sach != null)
            {
                var result = MessageBox.Show("Bạn có thực sự muốn xóa không?", "Cảnh báo", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    context.Saches.Remove(sach);
                    context.SaveChanges();
                    LoadData();
                    MessageBox.Show("OK Đã Xóa thành công");
                }
            }
            else
            {
                MessageBox.Show("Sách cần xóa không còn tồn tại!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSach.Text) ||
                string.IsNullOrEmpty(txtTenSach.Text) ||
                string.IsNullOrEmpty(txtNamXB.Text) ||
                      cmbLoaiSach.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sách!");
                return;
            }

            if (txtMaSach.Text.Length != 6)
            {
                MessageBox.Show("Mã sách phải có từ 6 kí tự trở lên!");
                return;
            }

            var maSach = txtMaSach.Text;
            var sach = context.Saches.FirstOrDefault(s => s.MaSach == maSach);

            if (sach == null)
            {
                sach = new Sach
                {
                    MaSach = txtMaSach.Text,
                    TenSach = txtTenSach.Text,
                    NamXB = int.Parse(txtNamXB.Text),
                    MaLoai = (int)cmbLoaiSach.SelectedValue
                };
                context.Saches.Add(sach);
                context.SaveChanges();
                MessageBox.Show("Thêm mới thành công");
            }
            else
            {
                sach.TenSach = txtTenSach.Text;
                sach.NamXB = int.Parse(txtNamXB.Text);
                sach.MaLoai = (int)cmbLoaiSach.SelectedValue;
                context.SaveChanges();
                MessageBox.Show("Cập nhật thành công");
            }

            LoadData();
            ResetForm();
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var searchTerm = txtSearch.Text;
            var sachList = context.Saches
                .Include(s => s.LoaiSach)
                .Where(s => s.MaSach.Contains(searchTerm) ||
                            s.TenSach.Contains(searchTerm) ||
                            s.NamXB.ToString().Contains(searchTerm))
                .ToList();
            BindGrid(sachList);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtMaSach.Text) ||
                string.IsNullOrEmpty(txtTenSach.Text) ||
                string.IsNullOrEmpty(txtNamXB.Text) ||
                cmbLoaiSach.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sách!");
                return;
            }

            if (txtMaSach.Text.Length != 6)
            {
                MessageBox.Show("Mã sách phải có 6 kí tự!");
                return;
            }

            var maSach = txtMaSach.Text;
            var sach = context.Saches.FirstOrDefault(s => s.MaSach == maSach);

            if (sach != null)
            {
                sach.TenSach = txtTenSach.Text;
                sach.NamXB = int.Parse(txtNamXB.Text);
                sach.MaLoai = (int)cmbLoaiSach.SelectedValue;
                context.SaveChanges();
                MessageBox.Show("Cập nhật thành công");
            }
            else
            {
                MessageBox.Show("Sách cần sửa không tồn tại!");
            }

            LoadData();
            ResetForm();
        }

        private void ResetForm()
        {
            txtMaSach.Clear();
            txtTenSach.Clear();
            txtNamXB.Clear();
            cmbLoaiSach.SelectedIndex = -1;
        }

    
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSach.Rows[e.RowIndex];
                txtMaSach.Text = row.Cells["MaSach"].Value.ToString();
                txtTenSach.Text = row.Cells["TenSach"].Value.ToString();
                txtNamXB.Text = row.Cells["NamXB"].Value.ToString();
                cmbLoaiSach.Text = row.Cells["TenLoai"].Value.ToString();
            }
        }
    }
}
