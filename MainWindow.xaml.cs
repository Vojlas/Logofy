using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Logofy
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] names = { "Výchozí", "Katedra hudební výchovy", "Katedra andragogiky a managementu vzdělávání", "Katedra dějin a didaktiky dějepisu", "Katedra občanské výchovy a filozofie", "Katedra pedagogiky", "Katedra preprimární a primární pedagogiky", "Katedra psychologie", "Katedra speciální pedagogiky", "Katedra tělesné výchovy", "Katedra výtvarné výchovy", "Katedra rustiky a lingvodidaktiky", "Katedra germanistiky", "Katedra francouzkého jazyka a literatury", "Katedra českého jazyka", "Katedra české literatury", "Katedra anglického jazyka a literatury", "Katedra biologie a enviromentálních studií", "Katedra chemie a didaktiky chemie", "Katedra informačních technologií a technické výchovy", "Katedra matematiky a didaktiky matematiky", "Ústav výzkumu a rozvoje vzdělání", "Knihovna" };
        
        List<Button> btns = new List<Button>();
        int selectedKatedra = 0;
        string name = "";
        string subjectName = "";
        string workName = "";
        string ImportPath = "";
        string ImportExtension = "";
        string fileName = "";
        bool toggleMenu = false;

        public MainWindow()
        {
            InitializeComponent();
            //GenerateCommonIMG();
            //Environment.Exit(0);

            for (int i =0; i<names.Length; i++)
            {                
                Button rbtn = new Button();
                rbtn.Content = names[i];
                rbtn.Tag = i;
                rbtn.Click += new RoutedEventHandler(btnBtnRbtn_Click);
                rbtn.HorizontalContentAlignment = HorizontalAlignment.Left;
                rbtn.Padding = new Thickness(5, 3.5, 0, 3.5);
                rbtn.MinWidth = 50;
                btns.Add(rbtn);

                ButtonsPlace.Children.Add(rbtn);
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Word Documents|*.doc;*.docx" + 
                                    "|Pdf Documents|*.pdf;" +
                                    "|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                lblFile.Content = openFileDialog.FileName;
                //Document dc = new Document(openFileDialog.FileName);
                //dc.Save("tmp.pdf");                
                ImportExtension = openFileDialog.FileName.EndsWith(".pdf") ? "pdf" : "docx";
                ImportPath = openFileDialog.FileName;
            }
        }

        private void btnBtnRbtn_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            foreach (Button btn in btns)
            {
                btn.ClearValue(Button.BackgroundProperty);
                btn.ClearValue(Button.FontWeightProperty);
            }
            b.Background = Brushes.Lavender;
            b.FontWeight = FontWeights.Bold;
            selectedKatedra = (int)b.Tag;
        }

        static void MergePDFs(string[] files, string filename)
        {
            // Open the output document
            PdfDocument outputDocument = new PdfDocument();

            // Iterate files
            foreach (string file in files)
            {
                // Open the document to import pages from it.
                PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                // Iterate pages
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    // Get the page from the external document...
                    PdfPage page = inputDocument.Pages[idx];
                    // ...and add it to the output document.
                    outputDocument.AddPage(page);
                }
            }
            outputDocument.Save(filename);
        }

        private void btnAbout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }
    }
}

