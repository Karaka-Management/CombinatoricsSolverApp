using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CombinatoricsSolverApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int minCombinations = 1;

        public static int maxCombinations = 6;

        public static double maxError = 0.005;

        public static int maxSolutions = -1; // -1 = all possible solutions
    }
}
