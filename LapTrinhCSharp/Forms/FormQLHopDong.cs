﻿using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using EzLife;
using HopDong = LapTrinhCSharp.Models.HopDong;
using Phong = LapTrinhCSharp.Models.Phong;
using SinhVienDB = LapTrinhCSharp.Models.SinhVien;

namespace LapTrinhCSharp.Forms
{
    public partial class FormQLHopDong : Form
    {
        public FormQLHopDong()
        {
            InitializeComponent();
        }

        private void LoadCombobox()
        {
            cmbPhong.DisplayMember = "Ten";
            cmbPhong.ValueMember = "ID";
            cmbPhong.DataSource = Phong.All();

            cmbSV.DataSource = SinhVienDB.All();
            cmbSV.DisplayMember = "HoTen";
            cmbSV.ValueMember = "MaSV";
        }

        private void LoadGrid()
        {
            grid.AutoGenerateColumns = false;
            grid.DataSource = HopDong.All();
        }

        private void FormQLHopDong_Load(object sender, EventArgs e)
        {
            LoadCombobox();
            LoadGrid();
        }

        private HopDong FormData
        {
            get {
                var hd = new HopDong
                {
                    MaHopDong = txtMaHD.Text,
                    SinhVien = (string)cmbSV.SelectedValue,
                    Phong = (int)cmbPhong.SelectedValue,
                    NgayBatDau = dtp1.Value,
                    NgayHetHan = dtp2.Value,
                    DaKetThuc = chk.Checked
                };
                return hd;
            }
        }

        private void grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string name = grid.Columns[e.ColumnIndex].Name;
            if (e.Value == null) return;
            if (name == "DaHetHan")
            {
                if (e.Value.ToString() == "False")
                {
                    e.Value = "Chưa hết hạn";
                }
                else
                {
                    e.Value = "Đã hết hạn";
                }
            }
            else if (name == "DaKetThuc")
            {
                if (e.Value.ToString() == "False")
                {
                    e.Value = "Chưa kết thúc";
                }
                else
                {
                    e.Value = "Đã kết thúc";
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var hd = FormData;
                hd.Create();
                LoadGrid();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
                return;
            try
            {
                var hd = FormData;
                hd.Update();
                LoadGrid();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {

            if (grid.SelectedRows.Count == 0)
                return;
            try
            {
                var hd = FormData;
                hd.Delete();
                LoadGrid();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            var row = grid.SelectedRows[0];
            txtMaHD.Text = row.Cells["MaHopDong"].Value.ToString();
            dtp1.Value = (DateTime)row.Cells["NgayBatDau"].Value;
            dtp2.Value = (DateTime)row.Cells["NgayHetHan"].Value;
            cmbPhong.SelectedValue = (int)row.Cells["MaPhong"].Value;
            cmbSV.SelectedValue = row.Cells["MaSV"].Value;
            chk.Checked = (bool)row.Cells["DaKetThuc"].Value;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            (new FormReportHopDong()).ShowDialog();
        }

        private void btnSearchSV_Click(object sender, EventArgs e)
        {
            var form = new FormTimKiemSV();
            form.ShowDialog();
            if (form.SelectedItem != null)
            {
                cmbSV.SelectedValue = form.SelectedItem;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            var id = FormData.MaHopDong;
            using (var db = new DbKTX())
            {
                var hd = db.HopDong.First(o => o.MaHopDong == id);
                var doc = Novacode.DocX.Load("hopdong.docx");
                doc.ReplaceText("{name}", hd.SinhVien.HoTen);
                doc.ReplaceText("{from}", hd.NgayBatDau.ToString("dd/MM/yyyy"));
                doc.ReplaceText("{to}", hd.NgayHetHan.ToString("dd/MM/yyyy"));
                doc.SaveAs("tmp.docx");
                var info = new ProcessStartInfo("tmp.docx");
                info.Verb = "Print";
                // info.CreateNoWindow = true;
                // info.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(info);
            }
        }

        private void btnSearchPhong_Click(object sender, EventArgs e)
        {
            var form = new FormQLPhong(true);
            form.ShowDialog();
            if (form.SelectedItem != null)
            {
                cmbPhong.SelectedValue = form.SelectedItem;
            }
        }

        private void cmbPhong_Format(object sender, ListControlConvertEventArgs e)
        {
            var row = (DataRowView) e.ListItem;
            e.Value = $"{row.Row.Field<string>("Nha")} : {row.Row.Field<string>("Ten")}";
        }
    }
}
