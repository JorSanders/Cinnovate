using FreeWheels.Classes;
using FreeWheels.PozyxLibrary.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Tests
{
    public class TestResult
    {

        public string TestCase { get; set; }
        public string Category { get; set; }
        public string Datetime { get; set; }
        public int TimeSpan { get; set; }

        public string[] Configurations { get; set; }

        public int TotalResults { get; set; }
        public int ZeroCount { get; set; }
        public Position[] Results { get; set; }

        public double Median { get; set; }
        public string Mode { get; set; }
        public double Average { get; set; }
        public double StandardDeviation { get; set; }

        public TestResult(){ }

        public TestResult(string testCase, string category)
        {
            this.TestCase = testCase;
            this.Category = category;
            this.Datetime = DateTime.Now.ToString();
        }

    }
}
