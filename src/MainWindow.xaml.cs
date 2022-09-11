using CombinatoricsSolverApp.Views;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;

namespace CombinatoricsSolverApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            #if OMS_DEMO
                this.Title = "Demo " + this.Title;
                MessageBox.Show("This is a demo with limited functionality.");
            #endif
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            Environment.Exit(0);
        }

        private void menuLoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "List files|*.xls;*.xlsx;*.csv"
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            int i = 0;
            int j = 0;
            switch (System.IO.Path.GetExtension(openFileDialog.FileName.ToLower()))
            {
                case ".xls":
                case ".xlsx":
                    SpreadsheetDocument doc = SpreadsheetDocument.Open(openFileDialog.FileName, false);
                    SheetData sheet = doc.WorkbookPart.WorksheetParts.First().Worksheet.Elements<SheetData>().First();

                    int rowCount = sheet.Elements<Row>().Count();

                    if (rowCount < 1)
                    {
                        return;
                    }

                    App.lines = new string[rowCount][];

                    i = 0;
                    int columnCount = 0;
                    
                    foreach (Row r in sheet.Elements<Row>())
                    {
                        #if OMS_DEMO
                            if (i > 30)
                            {
                                MessageBox.Show("Stopped reading file after 30 lines, consider to upgrade from the demo application to the full version.");
                                break;
                            }
                        #endif

                        if (i == 0)
                        {
                            columnCount = r.Elements().Count();
                        }

                        App.lines[i] = new string[columnCount];

                        j = 0;
                        foreach (Cell c in r.Elements<Cell>())
                        {
                            App.lines[i][j] = c.CellValue.Text;
                            ++j;
                        }

                        ++i;
                    }

                    break;
                case ".csv":
                    // Load text file
                    TextFieldParser parser = new TextFieldParser(openFileDialog.FileName);
                    parser.TextFieldType = FieldType.Delimited;

                    string[] lines = System.IO.File.ReadAllLines(openFileDialog.FileName);
                    if (lines.Length < 1)
                    {
                        return;
                    }

                    App.lines = new string[lines.Length][];

                    int commaCount     = lines[0].Split(",").Length - 1;
                    int semicolonCount = lines[0].Split(";").Length - 1;

                    if (commaCount > semicolonCount || commaCount == semicolonCount)
                    {
                        parser.SetDelimiters(",");
                    } else 
                    {
                        parser.SetDelimiters(";");
                    }

                    i = 0;
                    while (!parser.EndOfData)
                    {
                        #if OMS_DEMO
                            if (i > 30)
                            {
                                MessageBox.Show("Stopped reading file after 30 lines, consider to upgrade from the demo application to the full version.");
                                break;
                            }
                        #endif

                        string[]? fields = parser.ReadFields();
                        if (fields == null)
                        {
                            continue;
                        }

                        App.lines[i] = new string[fields.Length];
                        for (j = 0; j < fields.Length; ++j)
                        {
                            App.lines[i][j] = fields[j];
                        }

                        ++i;
                    }

                    break;
                default:
                    return;
            }
        }

        private void menuInfo_Click(object sender, RoutedEventArgs e)
        {
            if (Info.isOpen)
            {
                return;
            }

            Info window = new Info();
            window.Show();
        }

        private void menuSettings_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.isOpen)
            {
                return;
            }

            Settings window = new Settings(this);
            window.Show();
        }

        private void menuData_Click(object sender, RoutedEventArgs e)
        {
            if (Data.isOpen)
            {
                return;
            }

            Data window = new Data();
            window.Show();
        }
    }
}
