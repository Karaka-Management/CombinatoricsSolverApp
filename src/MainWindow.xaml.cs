using CombinatoricsSolverApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void menuLoadFile_Click(object sender, RoutedEventArgs e)
        {

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

            Settings window = new Settings();
            window.Show();
        }
    }
}
