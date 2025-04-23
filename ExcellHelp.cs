using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CrawFB.DAO
{
    public class ExcellHelp
    {
        private static ExcellHelp instance;

        public static ExcellHelp Instance
        {
            get { if (instance == null) instance = new ExcellHelp(); return ExcellHelp.instance; }
            private set { ExcellHelp.instance = value; }
        }



        // Hàm để tạo hoặc cập nhật file Excel
        public void DesignExcelFile(string header, string filePath, string SheetName)
        {
            if (File.Exists(filePath))
            {
                return; // Nếu file đã tồn tại, hàm sẽ kết thúc ngay
            }

            // Tạo mới workbook và worksheet
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(SheetName);

            // Thêm header
            var headers = header.Split(','); // Giả sử header là string, tách ra thành mảng
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            worksheet.Columns().AdjustToContents();
            // Lưu file
            workbook.SaveAs(filePath);
            MessageBox.Show("File Excel đã được tạo thành công, bắt đầu lưu?", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public int CompareLinkFbWithFile(string linkfb, string filePath)
        {
            if (!File.Exists(filePath))
            {
                return 0; // File không tồn tại
            }

            var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);

            // Duyệt qua các dòng trong cột linkfb (cột 2)
            for (int row = 2; !worksheet.Cell(row, 2).IsEmpty(); row++)
            {
                string existingLink = worksheet.Cell(row, 2).GetString().Trim();
                if (existingLink.Equals(linkfb.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return 1; // Nếu linkfb trùng thì trả về 1
                }
            }

            return 0; // Nếu không trùng, trả về 0
        }
        public void UpdateExcelPersonShare(string filePath, DataGridView dgv)
        {
            XLWorkbook workbook;
            IXLWorksheet worksheet;
            int currentRow = 2;

            // Nếu file tồn tại, mở file và tìm dòng trống tiếp theo
            if (File.Exists(filePath))
            {
                workbook = new XLWorkbook(filePath);
                worksheet = workbook.Worksheet(1);

                while (!worksheet.Cell(currentRow, 1).IsEmpty())
                {
                    currentRow++;
                }
            }
            else
            {
                MessageBox.Show("File không tồn tại!");
                return;
            }

            // Thêm dữ liệu từ DataGridView vào file Excel
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                string linkfb = row.Cells["linkfb"]?.Value?.ToString()?.Trim() ?? "";
                if (CompareLinkFbWithFile(linkfb, filePath) == 1) continue; // Nếu trùng linkfb thì bỏ qua

                string idfb = row.Cells["IdFb"]?.Value?.ToString() ?? "";
                string tenfb = row.Cells["DisplayName"]?.Value?.ToString() ?? "";
                string dentu = row.Cells["from"]?.Value?.ToString() ?? "";
                string noisong = row.Cells["live"]?.Value?.ToString() ?? "";
                string thongtinkhac = row.Cells["thongtinkhac"]?.Value?.ToString() ?? "";

                worksheet.Cell(currentRow, 1).Value = currentRow - 1; // STT
                worksheet.Cell(currentRow, 2).Value = linkfb;        // LinkFb
                worksheet.Cell(currentRow, 3).Value = idfb;          // ID Facebook
                worksheet.Cell(currentRow, 4).Value = tenfb;         // Tên Facebook
                worksheet.Cell(currentRow, 5).Value = dentu;         // Đến từ
                worksheet.Cell(currentRow, 6).Value = noisong;       // Sống tại
                worksheet.Cell(currentRow, 7).Value = thongtinkhac;  // Thông tin khác

                var bg = (currentRow % 2 == 0) ? XLColor.White : XLColor.LightCyan;
                for (int col = 1; col <= 7; col++)
                {
                    var cell = worksheet.Cell(currentRow, col);
                    cell.Style.Fill.BackgroundColor = bg;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }
                currentRow++;
                worksheet.Columns().AdjustToContents(); // Điều chỉnh độ rộng cuối cùng
            }

            // Lưu lại file Excel
            workbook.SaveAs(filePath);
            MessageBox.Show("Dữ liệu đã được thêm vào Excel!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public (string, string, string, string, string) GetDataByLink(string filePath, string linkfb)
        {
            string existingTenFb = string.Empty;
            string existingSongTai = string.Empty;
            string existingDenTu = string.Empty;
            string existingThongTinKhac = string.Empty;
            string existingIdFb = string.Empty;

            var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);

            // Duyệt qua các dòng trong worksheet để tìm linkfb trùng khớp
            for (int row = 2; !worksheet.Cell(row, 2).IsEmpty(); row++)
            {
                string existingLink = worksheet.Cell(row, 2).GetString().Trim();

                if (existingLink.Equals(linkfb.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    // Nếu linkfb trùng, lấy dữ liệu từ các cột khác
                    existingIdFb = worksheet.Cell(row, 3).GetString().Trim();
                    existingTenFb = worksheet.Cell(row, 4).GetString().Trim();                 
                    existingDenTu = worksheet.Cell(row, 5).GetString().Trim();
                    existingSongTai = worksheet.Cell(row, 6).GetString().Trim();
                    existingThongTinKhac = worksheet.Cell(row, 7).GetString().Trim();
                    break;  // Dừng vòng lặp khi tìm thấy
                }
            }

            return (existingIdFb, existingTenFb, existingSongTai, existingDenTu, existingThongTinKhac);
        }
    }
}
