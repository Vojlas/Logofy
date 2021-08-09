using ImageMagick;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Logofy
{
    partial class MainWindow : Window
    {
        private void btnGenerateCover_Click(object sender, RoutedEventArgs e)
        {
            generateMenuCover.Visibility = Visibility.Visible;
            fileName = workName + " - " + name + ".pdf";
        }
        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            fileName = workName + " - " + name + ".pdf";
            if (toggleMenu) { generateMenu.Visibility = Visibility.Collapsed; btnGenerate.Content = "Generate! ↓"; }
            else { generateMenu.Visibility = Visibility.Visible; btnGenerate.Content = "Generate! ↑"; }
            generateMenuCover.Visibility = Visibility.Collapsed;
            toggleMenu = !toggleMenu;

            name = txbUsrName.Text;
            workName = txbWorkName.Text;
            subjectName = txbSubName.Text;
        }
        /* -------------------- */

        private void btnGeneratePDF_Click(object sender, RoutedEventArgs e)
        {
            VisibilityHide();
            if (ImportExtension == "pdf")//extension PDF
            {
                PdfDocument pdfDocument;
                if (selectedKatedra != 0) pdfDocument = GenerateSpecialPDF();
                else pdfDocument = GenerateCommonPDF();
                pdfDocument.Save("tmp.pdf");

                using (PdfDocument one = PdfReader.Open(ImportPath, PdfDocumentOpenMode.Import))
                using(PdfDocument x = PdfReader.Open("tmp.pdf", PdfDocumentOpenMode.Import))
                using (PdfDocument outPdf = new PdfDocument())
                {
                    CopyPages(x, outPdf);
                    CopyPages(one, outPdf);

                    string name = UserSavePath(fileName, "pdf");
                    if (name == "") return;
                    outPdf.Save(name);
                }
                File.Delete("tmp.pdf");

                void CopyPages(PdfDocument from, PdfDocument to)
                {
                    for (int i = 0; i < from.PageCount; i++)
                    {
                        to.AddPage(from.Pages[i]);
                    }
                }
                MessageBox.Show("Your PDF was exported: " + fileName);
            }
        }
                
        private void btnGenerateCoverPDF_Click(object sender, RoutedEventArgs e)
        {
            VisibilityHide();

            PdfDocument pdfDocument;
            if (selectedKatedra != 0) pdfDocument = GenerateSpecialPDF();
            else pdfDocument = GenerateCommonPDF();
            string name = UserSavePath(fileName, "pdf");
            if (name == "") return;
            pdfDocument.Save(name);
            MessageBox.Show("Your cover(PDF) was exported: " + fileName);
        }

        private void btnGenerateWord_Click(object sender, RoutedEventArgs e)
        {
            VisibilityHide();
        }
        private void btnGenerateCoverImg_Click(object sender, RoutedEventArgs e)
        {
                     

            MessageBox.Show("Your cover(IMG) was exported: " + fileName);
        }       

        private Bitmap GenerateCommonIMG()
        {
            float mmpi = 25.4f; int dpi = 150;
            Bitmap canvas = new Bitmap((int)(210 / mmpi * dpi), (int)(297 / mmpi * dpi));
            canvas.SetResolution(dpi, dpi);

            using(Graphics gr = Graphics.FromImage(canvas))
            {
                gr.DrawImage(getLogo(0), new PointF(50, 50));
                gr.DrawRectangle(Pens.Black, new Rectangle(50, 50, 20, 20));
                gr.Flush();
            }



            canvas.Save("test.jpg");
            Process.Start("test.jpg");

            return canvas;
        }
        private PdfDocument GenerateCommonPDF()
        {            
            int Margin = 0;

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);
            XImage image = getLogoX(0);

            gfx.DrawImage(image, page.Width / 2 - 175, 50, 350, 350);

            XFont normalFont = new XFont("Calibri Light", 29, XFontStyle.Regular);
            Margin = WriteText(subjectName, ref gfx, ref page, normalFont, (int)page.Height / 2 + 70, XBrushes.Black, Margin);
            Margin = WriteText(name, ref gfx, ref page, normalFont, (int)page.Height / 2 + 150, XBrushes.Black, Margin);
            Margin = WriteText(workName, ref gfx, ref page, normalFont, (int)page.Height / 2 + 200, XBrushes.Black, Margin);

            return document;
        }
        private PdfDocument GenerateSpecialPDF()
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XImage image = getLogoX(selectedKatedra);
            int Margin = 0;

            gfx.DrawImage(image, page.Width / 2 - 125, 20, 250, 250);

            XFont font = new XFont("Georgia", 29, XFontStyle.Regular);
            XBrush redBrush = new XSolidBrush(XColor.FromArgb(210, 45, 64));
            Margin = WriteText("PEDAGOGICKÁ FAKULTA", ref gfx, ref page, font, (int)(page.Height / 4.0 + 70), XBrushes.Black, Margin, true);
            Margin = WriteText(names[selectedKatedra], ref gfx, ref page, font, (int)(page.Height / 4.0 + 120), XBrushes.Black, Margin, true);
            Margin = WriteText("Univerzita Karlova", ref gfx, ref page, font, (int)(page.Height / 4.0 + 170), redBrush, Margin, true);

            XFont normalFont = new XFont("Calibri Light", 29, XFontStyle.Regular);
            Margin = WriteText(subjectName, ref gfx, ref page, normalFont, (int)page.Height / 2 + 70, XBrushes.Black, Margin);
            Margin = WriteText(name, ref gfx, ref page, normalFont, (int)page.Height / 2 + 150, XBrushes.Black, Margin);
            Margin = WriteText(workName, ref gfx, ref page, normalFont, (int)page.Height / 2 + 200, XBrushes.Black, Margin);

            return document;
        }

        int WriteText(string text, ref XGraphics gfx, ref PdfPage page, XFont font, int X, XBrush color, int rowMargin = 0, bool header = false, int wordPerRow = 30) {
            int longMargin = rowMargin;
            if (text.Length < wordPerRow)
            {
                gfx.DrawString(text, font, color, new XRect(0, X + longMargin, page.Width, X + 30), XStringFormats.TopCenter);
            }
            else
            {
                string[] words = text.Split(null);
                int charCount = 0; int nrow = 1;
                List<string> Rows = new List<string>();
                string row = "";

                foreach (string wrd in words)
                {
                    charCount += wrd.Length;
                    if (charCount > wordPerRow * nrow)
                    {
                        nrow++;
                        Rows.Add(row);
                        row = wrd + " ";
                    }
                    else row += wrd + " ";
                }
                Rows.Add(row);

                foreach (var item in Rows)
                {
                    gfx.DrawString(item, font, color, new XRect(0, X + longMargin, page.Width, X + 30 + longMargin), XStringFormats.TopCenter);
                    if (Rows.IndexOf(item)+1 != Rows.Count)
                    {
                        if (header) { longMargin += 50; }
                        else longMargin += 30; 
                    }
                }
            }
            
            return longMargin;
        }

        private XImage getLogoX(int katedra)
        {
            var images = Properties.Resources.ResourceManager
                       .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                       .Cast<DictionaryEntry>()
                       .Where(x => x.Value.GetType() == typeof(System.Drawing.Bitmap))
                       .Select(x => new { Name = x.Key.ToString(), Image = x.Value })
                       .ToList();
            foreach (var item in images)
            {
                if ("_" + katedra == item.Name)
                {
                    var ms = new MemoryStream();
                    System.Drawing.Bitmap tmp = (System.Drawing.Bitmap)item.Image;
                    tmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return XImage.FromStream(ms);
                }
            }
            return null;
        }
        private Image getLogo(int katedra)
        {
            var images = Properties.Resources.ResourceManager
                       .GetResourceSet(CultureInfo.CurrentCulture, true, true)
                       .Cast<DictionaryEntry>()
                       .Where(x => x.Value.GetType() == typeof(System.Drawing.Bitmap))
                       .Select(x => new { Name = x.Key.ToString(), Image = x.Value })
                       .ToList();
            foreach (var item in images)
            {
                if ("_" + katedra == item.Name)
                {
                    var ms = new MemoryStream();
                    System.Drawing.Bitmap tmp = (System.Drawing.Bitmap)item.Image;
                    tmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return Image.FromStream(ms);
                }
            }
            return null;
        }
        private void VisibilityHide()
        {
            generateMenu.Visibility = Visibility.Collapsed;
            generateMenuCover.Visibility = Visibility.Collapsed;
            toggleMenu = !toggleMenu;
        }

        private string UserSavePath(string fileName, string ext)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = fileName; // Default file name
            dlg.DefaultExt = "."+ext; // Default file extension
            
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                return dlg.FileName;
            }
            return "";
        }
    }
}

