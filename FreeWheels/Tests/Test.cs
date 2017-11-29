using FreeWheels.PozyxLibrary;
using FreeWheels.PozyxLibrary.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace FreeWheels.Tests
{
    public class Test
    {
        private Pozyx _Pozyx;
        private Position position;
        public List<Position> PositionsList;
        public double[] DeviationsList;
        public int ZeroCount;
        public int TimeSpan;
        public string TestCase, Category;
        TestResult TestResult;

        public Test(Pozyx pozyx)
        {
            _Pozyx = pozyx;
            this.position = new Position();
        }

        public async void Export()
        {
            List<string> ExportData = new List<string>();
            ExportData.Add("");
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile sample = await folder.CreateFileAsync("dataExport.csv", CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteLinesAsync(sample, ExportData);
        }

        public async Task DoTest(int timeSpan, int interval, string testCase, string catagory)
        {
            this.TestCase = testCase;
            this.TimeSpan = timeSpan;

            DateTime stopTime = DateTime.Now.AddMilliseconds(timeSpan);
            ZeroCount = 0;

            while (DateTime.Now < stopTime)
            {
                int x = _Pozyx.PositioningData.PosX();
                int y = _Pozyx.PositioningData.PosY();
                int z = _Pozyx.PositioningData.PosZ();

                if (x == 0 && y == 0 && z == 0)
                {
                    ZeroCount++;
                }

                PositionsList.Add(new Position(x, y, z));
                await Task.Delay(interval);
            }
            CalculateDeviations();
            TestResult = GetTestResult();
        }

        public void CalculateDeviations()
        {
            double[] deviations = new double[PositionsList.Count];

            for (int i = 0; i < PositionsList.Count; i++)
            {
                // D² = A² + B² + C²
                deviations[i] = Math.Sqrt(Math.Pow(PositionsList[i].X, 2) + Math.Pow(PositionsList[i].Y, 2) + Math.Pow(PositionsList[i].Z, 2));
            }

            this.DeviationsList = deviations;
        }

        public double GetStandardDeviation()
        {
            int size = DeviationsList.Length;
            double total = DeviationsList.Sum();
            double average = total / size;

            double[] deviations = new double[size];

            // Calc Deviation on Average
            for (int i = 0; i < size; i++)
            {
                deviations[i] = Math.Pow(DeviationsList[i] - average, 2);
            }

            double deviationsTotal = deviations.Sum();
            double averageDeviation = deviationsTotal / size;
            double standardDeviation = Math.Sqrt(averageDeviation);

            return Math.Round(standardDeviation, 2);
        }

        public double GetAverage()
        {
            return Math.Round(this.DeviationsList.Sum() / this.DeviationsList.Length, 2);
        }

        public double GetMedian()
        {
            double[] deviations = this.DeviationsList;
            Array.Sort(deviations);

            if (deviations.Length % 2 == 0)
            {
                int index = deviations.Length / 2;
                return (deviations[index - 1] + deviations[index]) / 2;
            }
            else
            {
                int index = (int)((deviations.Length / 2) - 0.5);
                return deviations[index];
            }
        }

        public string GetMode()
        {
            Dictionary<double, int> counts = new Dictionary<double, int>();

            foreach (double deviation in this.DeviationsList)
            {
                if (counts.ContainsKey(deviation))
                {
                    counts[deviation] += 1;
                }
                else
                {
                    counts.Add(deviation, 1);
                }
            }

            Dictionary<double, int> mode = new Dictionary<double, int>();

            foreach (double key in counts.Keys)
            {
                if (mode.Count == 0)
                {
                    mode.Add(key, counts[key]);
                }
                else if (counts[key] == mode.First().Value)
                {
                    mode.Add(key, counts[key]);
                }
                else if (counts[key] > mode.First().Value)
                {
                    mode = new Dictionary<double, int>();
                    mode.Add(key, counts[key]);
                }
            }

            string str = "";

            foreach (double key in mode.Keys)
            {
                str += key + ", x" + mode[key];
            }

            return str;
        }

        public TestResult GetTestResult()
        {
            TestResult testResult = new TestResult(TestCase, Category);

            testResult.TimeSpan = TimeSpan;
            testResult.Configurations = new string[] { };

            testResult.TotalResults = this.PositionsList.Count();
            testResult.ZeroCount = this.ZeroCount;
            testResult.Results = this.PositionsList.ToArray();

            testResult.Median = GetMedian();
            testResult.Mode = GetMode(); ;
            testResult.Average = GetAverage();
            testResult.StandardDeviation = GetStandardDeviation();
            return testResult;
        }
    }
}
