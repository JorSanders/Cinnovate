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
    public class Testcase
    {
        private Pozyx _Pozyx;
        private Position MyPosition;
        public List<Position> PositionsList;
        public string[] Description;
        public int[] Deviations3d;
        public int[] Deviations2d;
        public int ZeroCount;
        public int TimeSpan;
        public string TestCase, Category;
        TestResult TestResult;
        DateTime startTime;

        /// <summary>
        ///     Creates a Testcase class. Call dotest to perform one
        /// </summary>
        /// <param name="pozyx">Pozyx object</param>
        public Testcase(Pozyx pozyx)
        {
            _Pozyx = pozyx;
        }

        /// <summary>
        ///     Export to csv
        /// </summary>
        /// <returns></returns>
        public async Task Export()
        {
            List<string> ExportData = new List<string>();
            ExportData.Add("sep=;");
            ExportData.Add("Testcase;" + TestResult.TestCase);
            ExportData.Add("Category;" + TestResult.Category);
            ExportData.Add("Datetime;" + TestResult.Datetime.ToString("dd MMMM yy H:mm"));
            ExportData.Add("Configurations:; ");
            foreach (string descritpionLine in TestResult.Description)
            {
                ExportData.Add(";" + descritpionLine + ";");
            }
            ExportData.Add("TimeSpan;" + TestResult.TimeSpan);
            ExportData.Add("TotalResults;" + TestResult.TotalResults);
            ExportData.Add("ZeroCount;" + TestResult.ZeroCount);
            ExportData.Add("");
            ExportData.Add("Info; 2d; 3d");
            ExportData.Add("Average;" + TestResult.Average2d + ";" + TestResult.Average3d);
            ExportData.Add("StandardDeviation;" + TestResult.StandardDeviation2d + ";" + TestResult.StandardDeviation3d);
            ExportData.Add("Median;" + TestResult.Median2d + ";" + TestResult.Median3d);
            ExportData.Add("Mode;" + TestResult.Mode2d + ";" + TestResult.Mode3d);
            ExportData.Add("");
            ExportData.Add("Results:; ");
            ExportData.Add(";X;Y;Z");
            foreach (Position position in TestResult.Results)
            {
                ExportData.Add(";" + position.X + ";" + position.Y + ";" + position.Z);
            }
            ExportData.Add("Deviations2d:; ");
            foreach (double deviation in TestResult.Deviations2d)
            {
                ExportData.Add(";" + deviation);
            }
            ExportData.Add("Deviations3d:; ");
            foreach (double deviation in TestResult.Deviations3d)
            {
                ExportData.Add(";" + deviation);
            }

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            string fileName = TestResult.TestCase + "-" + TestResult.Datetime.ToString("dd-MMMM-yy H-mm") + ".csv";
            StorageFile sample = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteLinesAsync(sample, ExportData);
        }

        /// <summary>
        ///     Perform the test. Automatically exports to pdf
        /// </summary>
        /// <param name="timeSpan">Timespan in ms the test will run</param>
        /// <param name="interval">Update interval</param>
        /// <param name="testCase">Name of testcase</param>
        /// <param name="catagory">catagory of the testcase</param>
        /// <returns></returns>
        public async Task DoTest(int timeSpan, int interval, string testCase, string catagory, string[] description)
        {
            this.MyPosition = new Position(2080, 340, 84);
            startTime = DateTime.Now;
            this.PositionsList = new List<Position>();
            this.TestCase = testCase;
            this.TimeSpan = timeSpan;
            this.Description = description;

            DateTime stopTime = DateTime.Now.AddMilliseconds(timeSpan);
            ZeroCount = 0;

            Position position;

            while (DateTime.Now < stopTime)
            {
                position = _Pozyx.PositioningData.Pos();

                if (position.X == 0 && position.Y == 0 && position.Z == 0)
                {
                    ZeroCount++;
                }

                PositionsList.Add(position);
                await Task.Delay(interval);
            }
            TestResult = UpdateTestResult();

            await this.Export();
        }

        /// <summary>
        ///     Calculates the devations of the test
        /// </summary>
        public void CalculateDeviations()
        {
            int[] deviations3d = new int[PositionsList.Count];
            int[] deviations2d = new int[PositionsList.Count];

            for (int i = 0; i < PositionsList.Count; i++)
            {
                double lineX = this.MyPosition.X - PositionsList[i].X;
                double lineY = this.MyPosition.Y - PositionsList[i].Y;
                double lineZ = this.MyPosition.Z - PositionsList[i].Z;

                // D² = A² + B² + C²
                deviations3d[i] = (int)Math.Sqrt(Math.Pow(lineX, 2) + Math.Pow(lineY, 2) + Math.Pow(lineZ, 2));

                // C² = A² + B²
                deviations2d[i] = (int)Math.Sqrt(Math.Pow(lineX, 2) + Math.Pow(lineY, 2));
            }

            this.Deviations3d = deviations3d;
            this.Deviations2d = deviations2d;
        }

        /// <summary>
        ///     Returns the standardevation of the test
        /// </summary>
        /// <returns></returns>
        public double GetStandardDeviation(int dimension)
        {
            int size;
            double total;

            switch (dimension)
            {
                case 2:
                    size = Deviations2d.Length;
                    total = Deviations2d.Sum();
                    break;
                case 3:
                    size = Deviations3d.Length;
                    total = Deviations3d.Sum();
                    break;
                default:
                    return -1;
            }

            double average = total / size;

            double[] deviations = new double[size];

            // Calc Deviation on Average
            for (int i = 0; i < size; i++)
            {
                deviations[i] = Math.Pow(Deviations3d[i] - average, 2);
            }

            double deviationsTotal = deviations.Sum();
            double averageDeviation = deviationsTotal / size;
            double standardDeviation = Math.Sqrt(averageDeviation);

            return Math.Round(standardDeviation, 2);
        }

        /// <summary>
        ///     Returns the average of the test
        /// </summary>
        /// <returns></returns>
        public double GetAverage(int dimension)
        {
            double average;
            switch (dimension)
            {
                case 2:
                    average = this.Deviations2d.Sum() / this.Deviations2d.Length;
                    return Math.Round(average, 2);
                case 3:
                    average = this.Deviations3d.Sum() / this.Deviations3d.Length;
                    return Math.Round(average, 2);
                default:
                    return -1;
            }
        }

        /// <summary>
        ///     Returns the median of the test
        /// </summary>
        /// <returns></returns>
        public double GetMedian(int dimension)
        {
            int[] deviations;

            switch (dimension)
            {
                case 2:
                    deviations = this.Deviations2d;
                    break;
                case 3:
                    deviations = this.Deviations3d;
                    break;
                default:
                    return -1;
            }

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

        /// <summary>
        ///     Returns the mode of the test
        /// </summary>
        /// <returns></returns>
        public string GetMode(int dimension)
        {
            Dictionary<double, int> counts = new Dictionary<double, int>();

            int[] deviations;

            switch (dimension)
            {
                case 2:
                    deviations = this.Deviations2d;
                    break;
                case 3:
                    deviations = this.Deviations3d;
                    break;
                default:
                    return null;
            }


            foreach (double deviation in deviations)
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

        /// <summary>
        ///     Sets the values in the testresult object
        /// </summary>
        /// <returns></returns>
        public TestResult UpdateTestResult()
        {
            CalculateDeviations();

            TestResult testResult = new TestResult(TestCase, Category);

            testResult.Datetime = this.startTime.ToLocalTime();
            testResult.TimeSpan = TimeSpan;
            testResult.Description = this.Description;

            testResult.TotalResults = this.PositionsList.Count();
            testResult.ZeroCount = this.ZeroCount;

            testResult.Results = this.PositionsList.ToArray();

            testResult.Deviations2d = this.Deviations2d;
            testResult.Median2d = GetMedian(2);
            testResult.Mode2d = GetMode(2); ;
            testResult.Average2d = GetAverage(2);
            testResult.StandardDeviation2d = GetStandardDeviation(2);

            testResult.Deviations3d = this.Deviations3d;
            testResult.Median3d = GetMedian(3);
            testResult.Mode3d = GetMode(3); ;
            testResult.Average3d = GetAverage(3);
            testResult.StandardDeviation3d = GetStandardDeviation(3);
            return testResult;
        }
    }
}
