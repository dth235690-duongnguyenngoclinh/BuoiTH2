using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyBanHang.Data; // Đảm bảo dòng này đúng namespace chứa Entity của bạn

namespace QuanLyBanHang.Forms
{
    public partial class frmHangSanXuat : Form
    {
        // 1. Khai báo context và biến _ID giống hệt mẫu
        private QLBHDbContext _context = new QLBHDbContext();
        private int _ID = 0; // _ID = 0 là Thêm, _ID > 0 là Sửa

        public frmHangSanXuat()
        {
            InitializeComponent();
        }

        private void HienThiDuLieu()
        {
            // Thay LoaiSanPham bằng HangSanXuat
            dataGridView.DataSource = _context.HangSanXuat.ToList();
        }

        // 2. Sự kiện Load: Nơi thực hiện yêu cầu KHÓA ô nhập liệu ban đầu
        private void frmHangSanXuat_Load(object sender, EventArgs e)
        {
            HienThiDuLieu();

            // Khóa nút Lưu/Hủy
            btnLuu.Enabled = false;
            btnHuyBo.Enabled = false;

            // Mở nút chức năng
            btnThem.Enabled = true;
            btnSua.Enabled = true;
            btnXoa.Enabled = true;

            // QUAN TRỌNG: Khóa ô nhập liệu ngay khi chạy form
            txtTenHangSanXuat.Enabled = false;
        }

        // 3. Sự kiện Click vào lưới để lấy dữ liệu
        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];

                // Lấy ID
                _ID = Convert.ToInt32(row.Cells["ID"].Value);

                // Gán tên hãng vào ô text (Lưu ý: Bạn phải đặt Name cột là TenHangSanXuat)
                txtTenHangSanXuat.Text = row.Cells["TenHangSanXuat"].Value.ToString();
            }
        }

        // 4. Nút Thêm: Mở khóa ô nhập liệu
        private void btnThem_Click(object sender, EventArgs e)
        {
            _ID = 0; // Đánh dấu là thêm mới

            // Xóa trắng
            txtTenHangSanXuat.Text = "";

            // QUAN TRỌNG: Mở khóa cho phép nhập
            txtTenHangSanXuat.Enabled = true;

            // Đảo trạng thái nút
            btnLuu.Enabled = true;
            btnHuyBo.Enabled = true;

            btnThem.Enabled = false;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            txtTenHangSanXuat.Focus();
        }

        // 5. Nút Sửa
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (_ID == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa trên lưới!");
                return;
            }

            // Mở khóa ô nhập
            txtTenHangSanXuat.Enabled = true;

            // Đảo trạng thái nút
            btnLuu.Enabled = true;
            btnHuyBo.Enabled = true;

            btnThem.Enabled = false;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
        }

        // 6. Nút Lưu
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenHangSanXuat.Text))
            {
                MessageBox.Show("Vui lòng nhập tên hãng sản xuất!");
                return;
            }

            // TRƯỜNG HỢP 1: THÊM MỚI
            if (_ID == 0)
            {
                HangSanXuat hsx = new HangSanXuat();
                hsx.TenHangSanXuat = txtTenHangSanXuat.Text;

                _context.HangSanXuat.Add(hsx);
                _context.SaveChanges();
                MessageBox.Show("Thêm mới thành công!");
            }
            // TRƯỜNG HỢP 2: SỬA
            else
            {
                var hsxCanSua = _context.HangSanXuat.Find(_ID);
                if (hsxCanSua != null)
                {
                    hsxCanSua.TenHangSanXuat = txtTenHangSanXuat.Text;
                    _context.SaveChanges();
                    MessageBox.Show("Cập nhật thành công!");
                }
            }

            // Tải lại lưới và Reset về trạng thái ban đầu (khóa ô nhập)
            HienThiDuLieu();
            frmHangSanXuat_Load(sender, e);
        }

        // 7. Nút Xóa
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (_ID == 0)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var hsxCanXoa = _context.HangSanXuat.Find(_ID);
                if (hsxCanXoa != null)
                {
                    _context.HangSanXuat.Remove(hsxCanXoa);
                    _context.SaveChanges();

                    MessageBox.Show("Đã xóa thành công!");
                    HienThiDuLieu();
                    frmHangSanXuat_Load(sender, e);
                }
            }
        }

        // 8. Nút Hủy
        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            txtTenHangSanXuat.Text = "";
            // Gọi lại Load để khóa ô nhập và reset nút
            frmHangSanXuat_Load(sender, e);
        }

        // 9. Nút Thoát
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}