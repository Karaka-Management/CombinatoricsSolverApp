using System.Windows;
using CombinatoricsSolverApp.Models;

namespace CombinatoricsSolverApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string VERSION = "1.0.0";

        public static int minCombinations = 1;

        public static int maxCombinations = 6;

        public static double maxError = 0.005;

        public static int maxSolutions = -1; // -1 = all possible solutions

        public static Lines[]? lines = null;
    }
}
