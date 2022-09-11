using System.Windows;

namespace CombinatoricsSolverApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string VERSION = "1.0.0";

        public static bool isFirstRun = true;

        public static double matchAmount = 0.0;

        public static int minCombinations = 1;

        public static int maxCombinations = 6;

        public static double maxError = 0.005;

        public static int maxSolutions = -1; // -1 = all possible solutions

        public static bool hasHeadline = true;

        public static int idColumn = 1;

        public static int dataColumn = 2;

        public static string[][]? lines = null;
    }
}
