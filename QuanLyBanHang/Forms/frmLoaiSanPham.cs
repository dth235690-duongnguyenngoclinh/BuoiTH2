using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using QuanLyBanHang.Data;
// Hoặc nếu tên namespace của em khác, hãy gõ "using QuanLy..." rồi chọn cái gợi ý có chữ Data.

namespace QuanLyBanHang.Forms
{
    public partial class frmLoaiSanPham : Form
    {
        // Khai báo biến ngữ cảnh Database (DbContext)
        private QLBHDbContext _context = new QLBHDbContext();
        private int _ID = 0;
        public frmLoaiSanPham()
        {
            InitializeComponent();
        }
        private void HienThiDuLieu()
        {

            // Lấy danh sách Loại sản phẩm từ Database và gán vào lưới
            dataGridView.DataSource = _context.LoaiSanPham.ToList();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void frmLoaiSanPham_Load(object sender, EventArgs e)
        {
            HienThiDuLieu();

            // Khóa các nút không cần thiết lúc đầu (theo yêu cầu đề bài)
            btnLuu.Enabled = false;
            btnHuyBo.Enabled = false;

            // Mở các nút chức năng
            btnThem.Enabled = true;
            btnSua.Enabled = true;
            btnXoa.Enabled = true;

            // Khóa ô nhập liệu
            txtTenLoai.Enabled = false;
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng có click vào dòng hợp lệ không
            if (e.RowIndex >= 0)
            {
                // Lấy dòng hiện tại
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];

                // Lấy ID lưu vào biến (Để lát nữa nút Sửa/Xóa biết mà dùng)
                _ID = Convert.ToInt32(row.Cells["ID"].Value);
                // Gán dữ liệu vào ô Textbox
                // Cells[0] là ID, Cells[1] là TenLoai (tùy thứ tự em đặt lúc nãy)
                txtTenLoai.Text = row.Cells["TenLoai"].Value.ToString();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            _ID = 0;
            // 1. Xóa trắng ô nhập tên
            txtTenLoai.Text = "";

            // 2. Mở khóa ô nhập cho phép gõ
            txtTenLoai.Enabled = true;

            // 3. Ẩn/Hiện các nút cho hợp lý
            btnLuu.Enabled= true;
            btnHuyBo.Enabled = true;

            btnThem.Enabled = false;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            // 4. Đưa con trỏ chuột vào ô nhập luôn cho tiện
            txtTenLoai.Focus();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenLoai.Text))
            {
                MessageBox.Show("Vui lòng nhập tên loại sản phẩm!");
                return;
            }

            // TRƯỜNG HỢP 1: THÊM MỚI (Khi _ID bằng 0)
            if (_ID == 0)
            {
                LoaiSanPham loaiMoi = new LoaiSanPham();
                loaiMoi.TenLoai = txtTenLoai.Text;
                _context.LoaiSanPham.Add(loaiMoi);
                _context.SaveChanges();
                MessageBox.Show("Thêm mới thành công!");
            }
            // TRƯỜNG HỢP 2: SỬA (Khi _ID khác 0)
            else
            {
                // Tìm dòng cần sửa trong Database dựa vào ID
                var loaiCanSua = _context.LoaiSanPham.Find(_ID);
                if (loaiCanSua != null)
                {
                    loaiCanSua.TenLoai = txtTenLoai.Text; // Gán tên mới
                    _context.SaveChanges(); // Lưu cập nhật
                    MessageBox.Show("Cập nhật thành công!");
                }
            }

            // Làm xong thì tải lại lưới và reset nút
            HienThiDuLieu();
            frmLoaiSanPham_Load(sender, e);
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            // Xóa trắng ô nhập
            txtTenLoai.Text = "";

            // Gọi lại hàm Load để reset trạng thái các nút
            frmLoaiSanPham_Load(sender, e);
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (_ID == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa trên lưới!");
                return;
            }

            // Mở khóa ô nhập
            txtTenLoai.Enabled = true;

            // Ẩn/Hiện nút
            btnLuu.Enabled = true;
            btnHuyBo.Enabled = true;

            btnThem.Enabled = false;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (_ID == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!");
                return;
            }

            // Hỏi xác nhận cho chắc ăn
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var loaiCanXoa = _context.LoaiSanPham.Find(_ID);
                if (loaiCanXoa != null)
                {
                    _context.LoaiSanPham.Remove(loaiCanXoa); // Lệnh xóa
                    _context.SaveChanges(); // Lưu xuống DB

                    MessageBox.Show("Đã xóa thành công!");
                    HienThiDuLieu();
                    frmLoaiSanPham_Load(sender, e);
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng Form hiện tại
        }
    }
}
