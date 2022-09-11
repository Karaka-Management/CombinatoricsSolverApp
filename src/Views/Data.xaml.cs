using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CombinatoricsSolverApp.Views
{
    /// <summary>
    /// Interaction logic for Data.xaml
    /// </summary>
    public partial class Data : Window
    {
        public static bool isOpen = false;

        public static Data? instance = null;

        public Data()
        {
            InitializeComponent();
            instance = this;
            isOpen   = true;

            this.Dispatcher.BeginInvoke(
                new ThreadStart(() => LoadDataGrid(gridData))
            );
        }

        public void LoadDataGrid(DataGrid gridData)
        {
            if (App.lines == null)
            {
                return;
            }

            // Clean up old grid
            gridData.Items.Clear();
            gridData.Items.Refresh();

            for (int i = gridData.Columns.Count - 1; i > -1; --i)
            {
                gridData.Columns.RemoveAt(i);
            }

            for (int i = 0; i < App.lines.Length; ++i)
            {
                if (i == 0)
                {
                    for (int j = 0; j < App.lines[i].Length; ++j)
                    {
                        DataGridTextColumn col = new DataGridTextColumn();
                        col.Header  = App.hasHeadline ? App.lines[i][j] : "Column " + j;
                        col.Binding = new Binding("[" + j + "]");

                        gridData.Columns.Add(col);
                    }

                    continue;
                }

                gridData.Items.Add(App.lines[i]);
            }           

            gridData.Items.Refresh();
        }

        private void DataWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isOpen   = false;
            instance = null;
        }

        private void DataClose_Click(object sender, RoutedEventArgs e)
        {
            isOpen   = false;
            instance = null;
            this.Close();
        }
    }
}
