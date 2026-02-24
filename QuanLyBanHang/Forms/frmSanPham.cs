using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyBanHang.Data; // Đảm bảo namespace này đúng với dự án của bạn

namespace QuanLyBanHang.Forms
{
    public partial class frmSanPham : Form
    {
        // 1. Khai báo biến toàn cục
        QLBHDbContext context = new QLBHDbContext();
        bool xuLyThem = false;
        int id;
        string imagesFolder = Application.StartupPath.Replace("bin\\Debug\\net5.0-windows", "Images");

        public frmSanPham()
        {
            InitializeComponent();
        }

        // 2. Hàm bật tắt chức năng
        private void BatTatChucNang(bool giaTri)
        {
            btnLuu.Enabled = giaTri;
            btnHuyBo.Enabled = giaTri;
            cboHangSanXuat.Enabled = giaTri;
            cboLoaiSanPham.Enabled = giaTri;
            txtTenSanPham.Enabled = giaTri;
            numSoLuong.Enabled = giaTri;
            numDonGia.Enabled = giaTri;
            txtMoTa.Enabled = giaTri;
            // picHinhAnh.Enabled = giaTri;

            btnThem.Enabled = !giaTri;
            btnDoiAnh.Enabled = !giaTri;
            btnSua.Enabled = !giaTri;
            btnXoa.Enabled = !giaTri;
        }

        // 3. Đổ dữ liệu vào ComboBox
        public void LayLoaiSanPhamVaoComboBox()
        {
            cboLoaiSanPham.DataSource = context.LoaiSanPham.ToList();
            cboLoaiSanPham.ValueMember = "ID";
            cboLoaiSanPham.DisplayMember = "TenLoai";
        }

        public void LayHangSanXuatVaoComboBox()
        {
            cboHangSanXuat.DataSource = context.HangSanXuat.ToList();
            cboHangSanXuat.ValueMember = "ID";
            cboHangSanXuat.DisplayMember = "TenHangSanXuat";
        }

        // 4. Tải Form
        private void frmSanPham_Load(object sender, EventArgs e)
        {
            BatTatChucNang(false);
            LayLoaiSanPhamVaoComboBox();
            LayHangSanXuatVaoComboBox();

            dataGridView.AutoGenerateColumns = false;

            // Lấy danh sách sản phẩm kèm thông tin loại và hãng
            var ds = context.SanPham.Select(r => new DanhSachSanPham
            {
                ID = r.ID,
                LoaiSanPhamID = r.LoaiSanPhamID,
                TenLoai = r.LoaiSanPham.TenLoai,
                HangSanXuatID = r.HangSanXuatID,
                TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                TenSanPham = r.TenSanPham,
                SoLuong = r.SoLuong,
                DonGia = r.DonGia,
                HinhAnh = r.HinhAnh,
                MoTa = r.MoTa
            }).ToList();

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = ds;

            // Clear và gán DataBindings
            cboLoaiSanPham.DataBindings.Clear();
            cboLoaiSanPham.DataBindings.Add("SelectedValue", bindingSource, "LoaiSanPhamID", false, DataSourceUpdateMode.Never);

            cboHangSanXuat.DataBindings.Clear();
            cboHangSanXuat.DataBindings.Add("SelectedValue", bindingSource, "HangSanXuatID", false, DataSourceUpdateMode.Never);

            txtTenSanPham.DataBindings.Clear();
            txtTenSanPham.DataBindings.Add("Text", bindingSource, "TenSanPham", false, DataSourceUpdateMode.Never);

            txtMoTa.DataBindings.Clear();
            txtMoTa.DataBindings.Add("Text", bindingSource, "MoTa", false, DataSourceUpdateMode.Never);

            numSoLuong.DataBindings.Clear();
            numSoLuong.DataBindings.Add("Value", bindingSource, "SoLuong", false, DataSourceUpdateMode.Never);

            numDonGia.DataBindings.Clear();
            numDonGia.DataBindings.Add("Value", bindingSource, "DonGia", false, DataSourceUpdateMode.Never);

            // Xử lý hiển thị hình ảnh khi chọn dòng
            picHinhAnh.DataBindings.Clear();
            Binding hinhAnhBinding = new Binding("ImageLocation", bindingSource, "HinhAnh", true);
            hinhAnhBinding.Format += (s, ev) =>
            {
                if (ev.Value != null)
                {
                    string path = Path.Combine(imagesFolder, ev.Value.ToString());
                    ev.Value = File.Exists(path) ? path : null;
                }
            };
            picHinhAnh.DataBindings.Add(hinhAnhBinding);

            dataGridView.DataSource = bindingSource;
        }

        // 5. Hiển thị ảnh icon lên DataGridView
        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].Name == "HinhAnh" && e.Value != null)
            {
                try
                {
                    string path = Path.Combine(imagesFolder, e.Value.ToString());
                    if (File.Exists(path))
                    {
                        using (Image image = Image.FromFile(path))
                        {
                            e.Value = new Bitmap(image, 24, 24);
                        }
                    }
                }
                catch { }
            }
        }

        // 6. Nút Thêm
        private void btnThem_Click(object sender, EventArgs e)
        {
            xuLyThem = true;
            BatTatChucNang(true);
            cboLoaiSanPham.SelectedIndex = -1;
            cboHangSanXuat.SelectedIndex = -1;
            txtTenSanPham.Clear();
            txtMoTa.Clear();
            numSoLuong.Value = 0;
            numDonGia.Value = 0;
            picHinhAnh.Image = null;
        }

        // 7. Nút Sửa
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow != null)
            {
                xuLyThem = false;
                id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value);
                BatTatChucNang(true);
            }
        }

        // 8. Nút Lưu
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (cboLoaiSanPham.SelectedValue == null)
                MessageBox.Show("Vui lòng chọn loại sản phẩm.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (cboHangSanXuat.SelectedValue == null)
                MessageBox.Show("Vui lòng chọn hãng sản xuất.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (string.IsNullOrWhiteSpace(txtTenSanPham.Text))
                MessageBox.Show("Vui lòng nhập tên sản phẩm.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (numSoLuong.Value <= 0)
                MessageBox.Show("Số lượng phải lớn hơn 0.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (numDonGia.Value <= 0)
                MessageBox.Show("Đơn giá sản phẩm phải lớn hơn 0.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if (xuLyThem)
                {
                    SanPham sp = new SanPham();
                    sp.TenSanPham = txtTenSanPham.Text;
                    sp.LoaiSanPhamID = (int)cboLoaiSanPham.SelectedValue;
                    sp.HangSanXuatID = (int)cboHangSanXuat.SelectedValue;
                    sp.SoLuong = (int)numSoLuong.Value;
                    sp.DonGia = (int)numDonGia.Value;
                    sp.MoTa = txtMoTa.Text;
                    context.SanPham.Add(sp);
                }
                else
                {
                    var sp = context.SanPham.Find(id);
                    if (sp != null)
                    {
                        sp.TenSanPham = txtTenSanPham.Text;
                        sp.LoaiSanPhamID = (int)cboLoaiSanPham.SelectedValue;
                        sp.HangSanXuatID = (int)cboHangSanXuat.SelectedValue;
                        sp.SoLuong = (int)numSoLuong.Value;
                        sp.DonGia = (int)numDonGia.Value;
                        sp.MoTa = txtMoTa.Text;
                    }
                }
                context.SaveChanges();
                frmSanPham_Load(sender, e);
            }
        }

        // 9. Nút Xóa
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow != null)
            {
                if (MessageBox.Show("Xác nhận xóa sản phẩm " + txtTenSanPham.Text + "?", "Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value);
                    var sp = context.SanPham.Find(id);
                    if (sp != null)
                    {
                        context.SanPham.Remove(sp);
                        context.SaveChanges();
                        frmSanPham_Load(sender, e);
                    }
                }
            }
        }

        // 10. Nút Hủy bỏ
        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            frmSanPham_Load(sender, e);
        }

        // 11. Nút Đổi ảnh
        private void btnDoiAnh_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Cập nhật hình ảnh sản phẩm";
            openFileDialog.Filter = "Tập tin hình ảnh|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileName(openFileDialog.FileName);
                string fileSavePath = Path.Combine(imagesFolder, fileName);

                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);

                File.Copy(openFileDialog.FileName, fileSavePath, true);

                id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value);
                var sp = context.SanPham.Find(id);
                if (sp != null)
                {
                    sp.HinhAnh = fileName;
                    context.SaveChanges();
                    frmSanPham_Load(sender, e);
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    // Class bổ trợ hiển thị danh sách
    [NotMapped]
    public class DanhSachSanPham
    {
        public int ID { get; set; }
        public int HangSanXuatID { get; set; }
        public string TenHangSanXuat { get; set; }
        public int LoaiSanPhamID { get; set; }
        public string TenLoai { get; set; }
        public string TenSanPham { get; set; }
        public int DonGia { get; set; }
        public int SoLuong { get; set; }
        public string? HinhAnh { get; set; }
        public string? MoTa { get; set; }
    }
}