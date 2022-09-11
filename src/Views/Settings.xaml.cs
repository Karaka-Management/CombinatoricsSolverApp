using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CombinatoricsSolverApp.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public static bool isOpen = false;

        private MainWindow main;

        public Settings(MainWindow main)
        {
            InitializeComponent();
            isOpen = true;

            this.main = main;
        }

        private void SettingsClose_Click(object sender, RoutedEventArgs e)
        {
            isOpen = false;
            this.Close();
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isOpen = false;
        }

        private void maxCombinations_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled   = regex.IsMatch(e.Text);

            #if OMS_DEMO
                if (e.Handled && Int32.Parse(e.Text) > 4) {
                    maxCombinations.Text = "4";
                }
            #endif
        }

        private void minCombinations_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void columnWithData_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void columnWithId_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SettingsRun_Click(object sender, RoutedEventArgs e)
        {
            if (App.lines == null)
            {
                return;
            }

            // save old values
            double matchAmountOld  = App.matchAmount;
            double maxErrorOld     = App.maxError;
            int minCombinationsOld = App.minCombinations;
            int maxCombinationsOld = App.maxCombinations;

            int idColumnOld     = App.idColumn;
            int dataColumnOld   = App.dataColumn;
            bool hasHeadlineOld = App.hasHeadline;

            // get new values
            App.matchAmount     = Double.Parse(amountToMatch.Text);
            App.maxError        = Math.Max(Double.Parse(allowedError.Text), 0.005);
            App.minCombinations = Int32.Parse(minCombinations.Text);
            App.maxCombinations = Int32.Parse(maxCombinations.Text);

            App.idColumn        = Int32.Parse(columnWithId.Text);
            App.dataColumn      = Int32.Parse(columnWithData.Text);
            App.hasHeadline     = hasHeadline.IsChecked == null || hasHeadline.IsLoaded;

            if (binomialCoefficient(App.lines.Length - (App.hasHeadline ? 1 : 0), App.maxCombinations) > 100_000_000_000) {
                MessageBox.Show(
                    "The data set or your settings result in too many possible combinations.\n"
                    + " Either reduce the data set or lower the maximum number of combinations"
                );

                return;
            }

            bool isDifferent = App.matchAmount != matchAmountOld
                || App.maxError != maxErrorOld
                || App.minCombinations != minCombinationsOld
                || App.maxCombinations != maxCombinationsOld
                || App.idColumn != idColumnOld
                || App.dataColumn != dataColumnOld
                || App.hasHeadline != hasHeadlineOld;

            // reload data grid if necessary
            if (isDifferent || App.isFirstRun)
            {
                App.isFirstRun = false;

                // sort data (not headline)
                string[][] firstLine = new string[1][] { App.lines[0] };
                IEnumerable<string[]> tempData = App.lines.Skip(1);

                string[][] sortedData = tempData.OrderBy(entry => Double.Parse(entry[App.dataColumn - 1])).ToArray();

                Array.Copy(firstLine, App.lines, 1);
                Array.Copy(sortedData, 0, App.lines, 1, sortedData.Length);

                // re-render data grid if visible
                if (Data.isOpen && Data.instance != null)
                {
                    this.Dispatcher.BeginInvoke(
                        new ThreadStart(() => Data.instance.LoadDataGrid(Data.instance.gridData))
                    );
                }
            }

            // Clean up old main grid
            main.mainDataGrid.Items.Clear();
            main.mainDataGrid.Items.Refresh();

            PropertyGroupDescription pgd = new PropertyGroupDescription("Id");
            main.mainDataGrid.Items.GroupDescriptions.Add(pgd);

            // @todo: find matches/combinations
            string[] ids  = new string[App.hasHeadline ? App.lines.Length - 1 : App.lines.Length];
            double[] data = new double[ids.Length];

            List<int[]>[] combinations = new List<int[]>[App.maxCombinations - App.minCombinations + 1];
            int j = 0;

            for (int i = App.hasHeadline ? 1 : 0; i < App.lines.Length; i++)
            {
                ids[j]  = App.lines[i][App.idColumn - 1];
                data[j] = Double.Parse(App.lines[i][App.dataColumn - 1]);

                ++j;
            }

            // @todo + 1 should not be here, but if i remove it, it goes out of bounds in the createCombinations
            int[] tempSolution = new int[App.maxCombinations];
            for (int i = App.minCombinations; i <= App.maxCombinations; ++i)
            {
                createCombinations(ref data, tempSolution, ref combinations, 0, data.Length - 1, 0, i);
            }

            int solutionSet = 0;
            for (int i = 0; i < combinations.Length; ++i)
            {
                if (combinations[i] == null)
                {
                    continue;
                }

                for (j = 0; j < combinations[i].Count; ++j) 
                {
                    ++solutionSet;

                    for (int k = 0; k < i + 1; ++k) {
                        main.mainDataGrid.Items.Add(new { 
                            Solution = solutionSet,
                            Id = App.lines[combinations[i].ElementAt(j)[k] + (App.hasHeadline ? 1 : 0)][App.idColumn - 1], 
                            Amount = App.lines[combinations[i].ElementAt(j)[k] + (App.hasHeadline ? 1 : 0)][App.dataColumn - 1]
                        });
                    }
                }
            }

            main.txtStats.Text = "Amount to Match: " + App.matchAmount.ToString("F2")
                + "; Error: " + App.maxError.ToString("F2")
                + "; Combinations: " + App.maxCombinations
                + "; Lines: " + (App.lines.Length - (App.hasHeadline ? 1 : 0))
                + "; Solutions: " + solutionSet;
        }

        private static long binomialCoefficient(long n, long k)
        {
            if (k > n)
            {
                return 0;
            }

            if (n == k)
            {
                return 1;
            }

            if (k > n - k)
            {
                k = n - k;
            }

            long c = 1;
            for (long i = 1; i <= k; ++i)
            {
                c *= --n;
                c /= i;
            }

            return c;
        }

        private void createCombinations(
            ref double[] data, 
            int[] tempSolution,
            ref List<int[]>[] combinations, 
            int start, int end, int index, 
            int currentCombinations = 1
        ){
            if (index == currentCombinations)
            {
                if (combinations[currentCombinations - 1] == null)
                {
                    combinations[currentCombinations - 1] = new List<int[]>();
                }

                double tmpAmount = 0.0;
                for (int i = 0; i < currentCombinations; ++i)
                {
                    tmpAmount += data[tempSolution[i]];
                }

                if (Math.Abs(tmpAmount - App.matchAmount) > App.maxError) {
                    return;
                }

                combinations[currentCombinations - 1].Add(tempSolution[0..currentCombinations]);

                return;
            }

            double amount = 0.0;
            for (int i = 0; i < index - 1; ++i)
            {
                if (amount - App.maxError > App.matchAmount)
                {
                    break;
                }

                amount += data[tempSolution[i]];
            }

            for (int i = start; i <= end && end - i + 1 >= currentCombinations - index; ++i)
            {
                if (amount + data[i] - App.maxError > App.matchAmount)
                {
                    break;
                }

                tempSolution[index] = i;
                createCombinations(
                    ref data,
                    tempSolution,
                    ref combinations,
                    i + 1, end, index + 1,
                    currentCombinations
                );
            }
        }
    }
}
