using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls.Ribbon;

namespace WpfApp
{
    public partial class MainWindow : RibbonWindow
    {
        public bool IsSaved = false;
        private string currentFilePath;

        public MainWindow()
        {
            InitializeComponent();
            fontSizeComboBox.ItemsSource = FontSizes;
        }

        public double[] FontSizes
        {
            get
            {
                return new double[] { 8.0, 9.0, 10.0, 11.0, 12.0, 14.0, 16.0, 18.0, 20.0, 22.0, 24.0, 26.0, 28.0, 36.0, 48.0, 72.0, 96.0, 144.0 };
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            doc1.Document.Blocks.Clear();
            currentFilePath = null;
            IsSaved = false;
            Title = "New Document - WPF Application";
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Document files (*.rtf)|*.rtf";
            var result = dlg.ShowDialog();
            if (result.Value)
            {
                TextRange t = new TextRange(doc1.Document.ContentStart, doc1.Document.ContentEnd);
                FileStream file = new FileStream(dlg.FileName, FileMode.Open);
                t.Load(file, System.Windows.DataFormats.Rtf);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = "unknown.doc";
            savefile.Filter = "Document files (*.rtf)|*.rtf";
            if (savefile.ShowDialog() == true)
            {
                TextRange t = new TextRange(doc1.Document.ContentStart, doc1.Document.ContentEnd);
                this.Title = this.Title + " " + savefile.FileName;
                FileStream file = new FileStream(savefile.FileName, FileMode.Create);
                t.Save(file, System.Windows.DataFormats.Rtf);
                file.Close();
            }
            IsSaved = true;
        }


        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument docToPrint = new FlowDocument();

                TextRange content = new TextRange(doc1.Document.ContentStart, doc1.Document.ContentEnd);

                using (MemoryStream stream = new MemoryStream())
                {
                    content.Save(stream, DataFormats.Xaml);
                    stream.Position = 0;
                    new TextRange(docToPrint.ContentStart, docToPrint.ContentEnd).Load(stream, DataFormats.Xaml);
                }
                IDocumentPaginatorSource idpSource = docToPrint;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Друк документа");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (IsSaved == false)
                if (MessageBox.Show("Do you want save changes ?", "Message", MessageBoxButton.YesNo) ==
               MessageBoxResult.Yes)
                {
                    this.btnSave_Click(sender, e);
                }
            this.Close();
        }

        private void FontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontFamilyComboBox.SelectedItem is FontFamily selectedFont)
            {
                doc1.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, selectedFont);
            }
        }

        private void FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontSizeComboBox.SelectedItem is double selectedSize)
            {
                doc1.Selection.ApplyPropertyValue(Inline.FontSizeProperty, selectedSize);
            }
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Help content goes here.", "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnChangeTextColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var selectedColor = new SolidColorBrush(Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));

                doc1.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, selectedColor);
            }
        }
    }
}
