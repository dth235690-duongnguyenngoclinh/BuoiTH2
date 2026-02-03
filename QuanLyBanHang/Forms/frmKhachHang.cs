using QuanLyBanHang.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHang.Forms
{
    public partial class frmKhachHang : Form
    {
        // 1. Sửa lại tên biến cho đúng chính tả và kiểu dữ liệu
        private QLBHDbContext context = new QLBHDbContext();
        private bool xuLyThem = false;
        int id = 0;

        public frmKhachHang()
        {
            InitializeComponent();
        }

        private void HienThiDuLieu()
        {
            // Tải lại dữ liệu mới nhất từ DB
            context = new QLBHDbContext();
            dataGridView.DataSource = context.KhachHang.ToList();
        }

        private void BatTatChucNang(bool giatri)
        {
            // Nhóm 1: Các ô nhập liệu và nút Lưu/Hủy đi cùng hướng với giatri
            btnLuu.Enabled = giatri;
            btnHuyBo.Enabled = giatri;
            txtHoVaTen.Enabled = giatri;
            txtDienThoai.Enabled = giatri;
            txtDiaChi.Enabled = giatri;

            // Nhóm 2: Các nút chức năng chính đi ngược với giatri
            btnThem.Enabled = !giatri;
            btnSua.Enabled = !giatri;
            btnXoa.Enabled = !giatri;
            btnThoat.Enabled = !giatri; // Thoát nên để mở khi không sửa
            // Thêm các nút khác của bạn vào đây nếu cần
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow != null)
            {
                xuLyThem = false; // Đánh dấu là đang SỬA
                BatTatChucNang(true);
                // Lấy ID của dòng đang chọn trên lưới
                id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoVaTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên khách hàng?", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (xuLyThem)
            {
                // Logic Thêm mới
                KhachHang kh = new KhachHang();
                kh.HoVaTen = txtHoVaTen.Text;
                kh.DienThoai = txtDienThoai.Text;
                kh.DiaChi = txtDiaChi.Text;
                context.KhachHang.Add(kh);
                context.SaveChanges();
            }
            else
            {
                // Logic Sửa: Tìm khách hàng trong DB bằng biến id đã lấy ở btnSua_Click
                var kh = context.KhachHang.Find(id);
                if (kh != null)
                {
                    kh.HoVaTen = txtHoVaTen.Text;
                    kh.DienThoai = txtDienThoai.Text;
                    kh.DiaChi = txtDiaChi.Text;
                    context.KhachHang.Update(kh);
                    context.SaveChanges();
                }
            }
            frmKhachHang_Load(sender, e);
            context.SaveChanges(); // Lưu vào CSDL
            MessageBox.Show("Đã lưu dữ liệu thành công!");

            // Gọi lại hàm Load để làm mới danh sách và khóa các nút
            frmKhachHang_Load(sender, e);
        }

        // Đừng quên hàm này để đồng bộ dữ liệu
        private void frmKhachHang_Load(object sender, EventArgs e)
        {
            BatTatChucNang(false);

            List<KhachHang> kh = new List<KhachHang>();
            kh = context.KhachHang.ToList();

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = kh;

            txtHoVaTen.DataBindings.Clear();
            txtHoVaTen.DataBindings.Add("Text", bindingSource, "HoVaTen", false, DataSourceUpdateMode.Never);

            // Tương tự cho txtDienThoai và txtDiaChi 
            txtDienThoai.DataBindings.Clear();
            txtDienThoai.DataBindings.Add("Text", bindingSource, "DienThoai", false, DataSourceUpdateMode.Never);
            txtDiaChi.DataBindings.Clear();
            txtDiaChi.DataBindings.Add("Text", bindingSource, "DiaChi", false, DataSourceUpdateMode.Never);

            dataGridView.DataSource = bindingSource;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xác nhận xóa khách hàng " + txtHoVaTen.Text + "?", "Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
                KhachHang kh = context.KhachHang.Find(id);
                if (kh != null)
                {
                    context.KhachHang.Remove(kh);
                    context.SaveChanges();
                }
                frmKhachHang_Load(sender, e);
            }
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            frmKhachHang_Load(sender, e);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            // Lấy từ khóa từ ô Họ và tên để tìm kiếm
            string tuKhoa = txtHoVaTen.Text.Trim();

            if (string.IsNullOrEmpty(tuKhoa))
            {
                // Nếu không nhập gì thì hiển thị lại toàn bộ danh sách
                HienThiDuLieu();
            }
            else
            {
                // Tìm kiếm các khách hàng có tên chứa từ khóa (không phân biệt hoa thường)
                var ketQua = context.KhachHang
                                    .Where(kh => kh.HoVaTen.Contains(tuKhoa))
                                    .ToList();

                dataGridView.DataSource = ketQua;

                if (ketQua.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy khách hàng nào phù hợp.", "Thông báo");
                }
            }
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            // Sử dụng SaveFileDialog để chọn nơi lưu file
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV File (*.csv)|*.csv";
            sfd.FileName = "DanhSachKhachHang.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Viết dữ liệu ra file theo định dạng CSV
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        // Viết tiêu đề cột
                        sw.WriteLine("ID,Họ và tên,Điện thoại,Địa chỉ");

                        // Duyệt qua danh sách khách hàng từ context
                        var danhSach = context.KhachHang.ToList();
                        foreach (var kh in danhSach)
                        {
                            sw.WriteLine($"{kh.ID},{kh.HoVaTen},{kh.DienThoai},{kh.DiaChi}");
                        }
                    }
                    MessageBox.Show("Xuất dữ liệu thành công!", "Thông báo");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất dữ liệu: " + ex.Message, "Lỗi");
                }
            }
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] lines = System.IO.File.ReadAllLines(ofd.FileName);
                    foreach (string line in lines)
                    {
                        // Giả sử file có định dạng: Họ tên,Điện thoại,Địa chỉ
                        string[] data = line.Split(',');
                        if (data.Length >= 3)
                        {
                            KhachHang kh = new KhachHang
                            {
                                HoVaTen = data[0].Trim(),
                                DienThoai = data[1].Trim(),
                                DiaChi = data[2].Trim()
                            };
                            context.KhachHang.Add(kh);
                        }
                    }
                    context.SaveChanges();
                    HienThiDuLieu();
                    MessageBox.Show("Nhập dữ liệu thành công!", "Thông báo");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi nhập dữ liệu: " + ex.Message, "Lỗi");
                }
            }
        }
    }
}
