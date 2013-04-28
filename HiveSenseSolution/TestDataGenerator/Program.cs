using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HiveSenseTest;

namespace TestDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            var r = new Reporter("18824523-d961-4f14-922c-6ea2783ffe31");
            r.ReportMetrics();


        }
    }
}
