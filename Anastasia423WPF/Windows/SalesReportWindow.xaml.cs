using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using WFC = System.Windows.Forms.DataVisualization.Charting;
using Drawing = System.Drawing;

namespace Anastasia423WPF.Windows
{
    public class SalesRow
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
    }

    public partial class SalesReportWindow : Window
    {
        private List<SalesRow> _rows = new List<SalesRow>();

        public SalesReportWindow()
        {
            InitializeComponent();
            LoadReport();
        }

        // ---------- ДАННЫЕ ----------

        private void LoadReport()
        {
            DateTime from = FromPicker.SelectedDate ?? new DateTime(2000, 1, 1);
            DateTime to = (ToPicker.SelectedDate ?? new DateTime(2100, 1, 1)).AddDays(1);

            // Продажей считаем выданные заказы
            var lines = Core.Context.Product_Order
                .Include(po => po.Product)
                .Include(po => po.Order)
                .Where(po => po.Order.Status == "Выдан"
                          && po.Order.OrderDate >= from
                          && po.Order.OrderDate < to)
                .ToList();

            _rows = lines
                .GroupBy(po => new { po.ProductID, po.Product.Name })
                .Select(g => new SalesRow
                {
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(x => x.CountProd),
                    Revenue = g.Sum(x => x.Product.Price * (1 - (decimal)x.Product.Discount / 100) * x.CountProd)
                })
                .OrderByDescending(r => r.Revenue)
                .ToList();

            SalesGrid.ItemsSource = _rows;
            TotalText.Text = $"Итого: {_rows.Sum(r => r.Quantity)} шт. на сумму {_rows.Sum(r => r.Revenue):N2} руб.";

            ChartHost.Child = BuildChart(_rows.Take(10).ToList(), 0, 0);
        }

        private string PeriodText()
        {
            if (FromPicker.SelectedDate == null && ToPicker.SelectedDate == null) return "за всё время";
            string f = FromPicker.SelectedDate?.ToString("dd.MM.yyyy") ?? "начала";
            string t = ToPicker.SelectedDate?.ToString("dd.MM.yyyy") ?? "сегодня";
            return $"с {f} по {t}";
        }

        private void Apply_Click(object sender, RoutedEventArgs e) => LoadReport();

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            FromPicker.SelectedDate = null;
            ToPicker.SelectedDate = null;
            LoadReport();
        }

        // ---------- ДИАГРАММА ----------

        // width/height = 0 -> диаграмма заполняет контейнер, иначе фиксированный размер (для картинки в Word)
        private WFC.Chart BuildChart(List<SalesRow> data, int width, int height)
        {
            var chart = new WFC.Chart { BackColor = Drawing.Color.White };
            if (width > 0) { chart.Width = width; chart.Height = height; }
            else chart.Dock = System.Windows.Forms.DockStyle.Fill;

            var area = new WFC.ChartArea("main");
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.Title = "Выручка, руб.";
            chart.ChartAreas.Add(area);

            chart.Titles.Add("Топ-10 товаров по выручке");

            var series = new WFC.Series("Выручка")
            {
                ChartType = WFC.SeriesChartType.Column,
                IsValueShownAsLabel = true,
                LabelFormat = "N0",
                Color = Drawing.Color.Crimson
            };
            foreach (var r in data)
            {
                string name = r.ProductName.Length > 18 ? r.ProductName.Substring(0, 18) + "…" : r.ProductName;
                series.Points.AddXY(name, (double)r.Revenue);
            }
            chart.Series.Add(series);
            return chart;
        }

        // ---------- ЭКСПОРТ В EXCEL ----------

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_rows.Count == 0) { MessageBox.Show("Нет данных для экспорта."); return; }

            var dlg = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"Продажи_{DateTime.Now:yyyy-MM-dd}.xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            Excel.Application app = null;
            Excel.Workbook wb = null;
            bool saved = false;
            try
            {
                app = new Excel.Application { Visible = false, DisplayAlerts = false };
                wb = app.Workbooks.Add();
                Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
                ws.Name = "Продажи";

                ws.Range["A1"].Value2 = "Отчёт по продажам товаров";
                ws.Range["A1"].Font.Bold = true;
                ws.Range["A1"].Font.Size = 14;
                ws.Range["A2"].Value2 = "Период: " + PeriodText();

                ws.Range["A4"].Value2 = "Товар";
                ws.Range["B4"].Value2 = "Количество, шт.";
                ws.Range["C4"].Value2 = "Выручка, руб.";
                Excel.Range head = ws.Range["A4", "C4"];
                head.Font.Bold = true;
                head.Interior.Color = Drawing.ColorTranslator.ToOle(Drawing.Color.LightGray);

                int r = 5;
                foreach (var row in _rows)
                {
                    ws.Range["A" + r].Value2 = row.ProductName;
                    ws.Range["B" + r].Value2 = row.Quantity;
                    ws.Range["C" + r].Value2 = (double)row.Revenue;
                    r++;
                }
                int last = r - 1;

                ws.Range["A" + r].Value2 = "Итого";
                ws.Range["B" + r].Formula = $"=SUM(B5:B{last})";
                ws.Range["C" + r].Formula = $"=SUM(C5:C{last})";
                ws.Range["A" + r, "C" + r].Font.Bold = true;

                ws.Range["C5", "C" + r].NumberFormat = "#,##0.00";
                ws.Range["A4", "C" + r].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                ws.Range["A:C"].EntireColumn.AutoFit();

                // Диаграмма (топ-10)
                int chartLast = Math.Min(last, 14);
                Excel.Range anchor = ws.Range["E4"];
                Excel.ChartObjects charts = (Excel.ChartObjects)ws.ChartObjects();
                Excel.ChartObject co = charts.Add((double)anchor.Left, (double)anchor.Top, 520, 320);
                Excel.Chart ch = co.Chart;
                ch.ChartType = Excel.XlChartType.xlColumnClustered;

                Excel.SeriesCollection sc = (Excel.SeriesCollection)ch.SeriesCollection();
                Excel.Series s = sc.NewSeries();
                s.Name = "Выручка, руб.";
                s.Values = ws.Range["C5", "C" + chartLast];
                s.XValues = ws.Range["A5", "A" + chartLast];
                ch.HasTitle = true;
                ch.ChartTitle.Text = "Топ-10 товаров по выручке";
                ch.HasLegend = false;

                wb.SaveAs(dlg.FileName, Excel.XlFileFormat.xlOpenXMLWorkbook);
                saved = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта в Excel: " + ex.Message);
            }
            finally
            {
                if (wb != null) { wb.Close(false); Marshal.ReleaseComObject(wb); }
                if (app != null) { app.Quit(); Marshal.ReleaseComObject(app); }
            }

            if (saved) AskOpen(dlg.FileName);
        }

        // ---------- ЭКСПОРТ В WORD ----------

        private void ExportWord_Click(object sender, RoutedEventArgs e)
        {
            if (_rows.Count == 0) { MessageBox.Show("Нет данных для экспорта."); return; }

            var dlg = new SaveFileDialog
            {
                Filter = "Word (*.docx)|*.docx",
                FileName = $"Продажи_{DateTime.Now:yyyy-MM-dd}.docx"
            };
            if (dlg.ShowDialog() != true) return;

            // Картинка диаграммы для вставки в документ
            string img = Path.Combine(Path.GetTempPath(), "sales_chart.png");
            using (var chart = BuildChart(_rows.Take(10).ToList(), 900, 450))
                chart.SaveImage(img, WFC.ChartImageFormat.Png);

            Word.Application app = null;
            Word.Document doc = null;
            bool saved = false;
            try
            {
                app = new Word.Application { Visible = false };
                doc = app.Documents.Add();
                Word.Selection sel = app.Selection;

                WriteLine(sel, "Отчёт по продажам товаров", 16, true, Word.WdParagraphAlignment.wdAlignParagraphCenter);
                WriteLine(sel, "Период: " + PeriodText(), 12, false, Word.WdParagraphAlignment.wdAlignParagraphCenter);
                WriteLine(sel, "Дата формирования: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"), 12, false, Word.WdParagraphAlignment.wdAlignParagraphCenter);
                WriteLine(sel, "", 12, false, Word.WdParagraphAlignment.wdAlignParagraphLeft);

                // Таблица
                Word.Table tbl = doc.Tables.Add(sel.Range, _rows.Count + 2, 3);
                tbl.Borders.Enable = 1;
                tbl.Range.Font.Size = 11;

                tbl.Cell(1, 1).Range.Text = "Товар";
                tbl.Cell(1, 2).Range.Text = "Количество, шт.";
                tbl.Cell(1, 3).Range.Text = "Выручка, руб.";
                tbl.Rows[1].Range.Font.Bold = 1;

                for (int i = 0; i < _rows.Count; i++)
                {
                    tbl.Cell(i + 2, 1).Range.Text = _rows[i].ProductName;
                    tbl.Cell(i + 2, 2).Range.Text = _rows[i].Quantity.ToString();
                    tbl.Cell(i + 2, 3).Range.Text = _rows[i].Revenue.ToString("N2");
                }
                int lastRow = _rows.Count + 2;
                tbl.Cell(lastRow, 1).Range.Text = "Итого";
                tbl.Cell(lastRow, 2).Range.Text = _rows.Sum(r => r.Quantity).ToString();
                tbl.Cell(lastRow, 3).Range.Text = _rows.Sum(r => r.Revenue).ToString("N2");
                tbl.Rows[lastRow].Range.Font.Bold = 1;

                // Диаграмма после таблицы
                sel.EndKey(Word.WdUnits.wdStory);
                WriteLine(sel, "", 12, false, Word.WdParagraphAlignment.wdAlignParagraphLeft);
                WriteLine(sel, "Диаграмма: топ-10 товаров по выручке", 12, true, Word.WdParagraphAlignment.wdAlignParagraphCenter);
                sel.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                Word.InlineShape pic = sel.InlineShapes.AddPicture(img);
                pic.Width = 450f;
                pic.Height = 225f;

                doc.Content.Font.Name = "Times New Roman";

                doc.SaveAs2(dlg.FileName, Word.WdSaveFormat.wdFormatXMLDocument);
                saved = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта в Word: " + ex.Message);
            }
            finally
            {
                if (doc != null) { ((Word._Document)doc).Close(false); Marshal.ReleaseComObject(doc); }
                if (app != null) { ((Word._Application)app).Quit(false); Marshal.ReleaseComObject(app); }
                try { File.Delete(img); } catch { }
            }

            if (saved) AskOpen(dlg.FileName);
        }

        private static void WriteLine(Word.Selection sel, string text, int size, bool bold, Word.WdParagraphAlignment align)
        {
            sel.ParagraphFormat.Alignment = align;
            sel.Font.Size = size;
            sel.Font.Bold = bold ? 1 : 0;
            sel.TypeText(text);
            sel.TypeParagraph();
        }

        private void AskOpen(string path)
        {
            if (MessageBox.Show("Файл сохранён. Открыть его?", "Экспорт", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
    }
}