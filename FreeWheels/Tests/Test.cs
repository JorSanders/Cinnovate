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
        private List<Position> PositionsList;
        private string[] Description;
        private int[] Deviations3d;
        private int[] Deviations2d;
        private int[] Distance2dList;
        private int[] Distance3dList;
        private int ZeroCount;
        private int TimeSpan;
        private string TestCase, Category;
        private TestResult TestResult;
        private DateTime startTime;
        public string Status;

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
            ExportData.Add(";X;Y;Z");
            ExportData.Add("Real position;" + TestResult.realPostion.X + ";" + TestResult.realPostion.Y + ";" + TestResult.realPostion.Z);
            ExportData.Add("Avg position;" + TestResult.avgPostion.X + ";" + TestResult.avgPostion.Y + ";" + TestResult.avgPostion.Z);
            ExportData.Add("");
            ExportData.Add(";deviation to avg 2d;deviation to avg 3d;deviation to real 2d;deviation to real 3d");
            ExportData.Add("Average;" + TestResult.AverageDistance2d + ";" + TestResult.AverageDistance3d + ";" + TestResult.Average2d + ";" + TestResult.Average3d);
            ExportData.Add("StandardDeviation;" + TestResult.StandardDeviationDistance2d + ";" + TestResult.StandardDeviationDistance3d + ";" + TestResult.StandardDeviation2d + ";" + TestResult.StandardDeviation3d);
            ExportData.Add("Median;" + TestResult.MedianDistance2d + ";" + TestResult.MedianDistance3d + ";" + TestResult.Median2d + ";" + TestResult.Median3d);
            ExportData.Add("Mode;" + TestResult.ModeDistance2d + ";" + TestResult.ModeDistance3d + ";" + TestResult.Mode2d + ";" + TestResult.Mode3d);
            ExportData.Add("");
            ExportData.Add("Results:; ");
            ExportData.Add(";X;Y;Z;deviation to avg 2d;deviation to avg 3d;deviation to real 2d;deviation to real 3d");
            for (int i = 0; i < TestResult.Results.Length; i++)
            {
                ExportData.Add(";" + TestResult.Results[i].X + ";" + TestResult.Results[i].Y + ";" + TestResult.Results[i].Z
                    + ";" + Distance2dList[i] + ";" + Distance3dList[i]
                    + ";" + Deviations2d[i] + ";" + Deviations3d[i]);
            }

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            string fileName = TestResult.TestCase + " " + TestResult.Datetime.ToString("dd-MMMM-yy H-mm") + ".csv";
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
        public async Task DoTest(int timeSpan, int interval, string testCase, string category, string[] description)
        {
            this.Status = "Setting configurations";

            await _Pozyx.SetConfiguration();

            this.MyPosition = new Position(2050, 1020, 840);
            startTime = DateTime.Now;
            this.PositionsList = new List<Position>();
            this.TestCase = testCase;
            this.TimeSpan = timeSpan;
            this.Description = description;
            this.Category = category;

            DateTime stopTime = DateTime.Now.AddMilliseconds(timeSpan);
            TimeSpan difference;

            ZeroCount = 0;

            Position position;

            while (DateTime.Now < stopTime)
            {
                difference = (stopTime - DateTime.Now);
                this.Status = difference.Minutes + ":" + difference.Seconds;
                position = _Pozyx.PositioningData.Pos();

                if (position.X == 0 && position.Y == 0 && position.Z == 0)
                {
                    ZeroCount++;
                }

                PositionsList.Add(position);
                await Task.Delay(interval);
            }
            this.Status = "Exporting";

            TestResult = UpdateTestResult();
            await this.Export();
        }

        /// <summary>
        ///     Calculates the devations of the test
        /// </summary>
        public void CalculateDeviations(Position avgPosition)
        {
            Deviations3d = new int[PositionsList.Count];
            Deviations2d = new int[PositionsList.Count];
            Distance2dList = new int[PositionsList.Count];
            Distance3dList = new int[PositionsList.Count];

            int realDistance2d = (int)Math.Round(Math.Sqrt(Math.Pow(MyPosition.X, 2) + Math.Pow(MyPosition.Y, 2)));
            int realDistance3d = (int)Math.Round(Math.Sqrt(Math.Pow(MyPosition.X, 2) + Math.Pow(MyPosition.Y, 2) + Math.Pow(MyPosition.Z, 2)));


            int avgDistance2d = (int)Math.Round(Math.Sqrt(Math.Pow(avgPosition.X, 2) + Math.Pow(avgPosition.Y, 2)));
            int avgDistance3d = (int)Math.Round(Math.Sqrt(Math.Pow(avgPosition.X, 2) + Math.Pow(avgPosition.Y, 2) + Math.Pow(avgPosition.Z, 2)));

            for (int i = 0; i < PositionsList.Count; i++)
            {
                // C² = A² + B²
                // D² = A² + B² + C²

                Distance2dList[i] = avgDistance2d - (int)Math.Round(Math.Sqrt(Math.Pow(PositionsList[i].X, 2) + Math.Pow(PositionsList[i].Y, 2)));
                Distance3dList[i] = avgDistance3d - (int)Math.Round(Math.Sqrt(Math.Pow(PositionsList[i].X, 2) + Math.Pow(PositionsList[i].Y, 2) + Math.Pow(PositionsList[i].Z, 2)));

                Deviations2d[i] = realDistance2d - (int)Math.Round(Math.Sqrt(Math.Pow(Distance2dList[i], 2) + Math.Pow(Distance2dList[i], 2)));
                Deviations3d[i] = realDistance3d - (int)Math.Round(Math.Sqrt(Math.Pow(Distance2dList[i], 2) + Math.Pow(Distance2dList[i], 2) + Math.Pow(Distance2dList[i], 2)));

                if (Distance2dList[i] < 0) Distance2dList[i] *= -1;
                if (Distance3dList[i] < 0) Distance3dList[i] *= -1;
                if (Deviations2d[i] < 0) Deviations2d[i] *= -1;
                if (Deviations3d[i] < 0) Deviations3d[i] *= -1;
            }
        }

        /// <summary>
        ///     Returns the standardevation of the test
        /// </summary>
        /// <returns></returns>
        public double GetStandardDeviation(int[] list)
        {
            int size = list.Length;
            double total = list.Sum();

            double average = total / size;

            double[] deviations = new double[size];

            // Calc Deviation on Average
            for (int i = 0; i < size; i++)
            {
                deviations[i] = Math.Pow(list[i] - average, 2);
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
        public double GetAverage(int[] list)
        {
            double average;
            average = list.Sum() / list.Length;
            return Math.Round(average, 2);
        }

        /// <summary>
        ///     Returns the median of the test
        /// </summary>
        /// <returns></returns>
        public double GetMedian(int[] list)
        {
            List<int> listList = list.ToList();
            int[] sortableList = listList.ToArray();
            Array.Sort(sortableList);

            if (sortableList.Length % 2 == 0)
            {
                int index = sortableList.Length / 2;
                return (sortableList[index - 1] + sortableList[index]) / 2;
            }
            else
            {
                int index = (int)Math.Round((sortableList.Length / 2) - 0.5);
                return sortableList[index];
            }
        }

        /// <summary>
        ///     Returns the mode of the test
        /// </summary>
        /// <returns></returns>
        public string GetMode(int[] list)
        {
            Dictionary<double, int> counts = new Dictionary<double, int>();

            foreach (double item in list)
            {
                if (counts.ContainsKey(item))
                {
                    counts[item] += 1;
                }
                else
                {
                    counts.Add(item, 1);
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

        public Position GetAveragePostion(List<Position> positions)
        {
            int totalX = 0, totalY = 0, totalZ = 0;
            int size = positions.Count;

            for (int i = 0; i < size; i++)
            {
                totalX += positions[i].X;
                totalY += positions[i].Y;
                totalZ += positions[i].Z;
            }

            return new Position(totalX / size, totalY / size, totalZ / size);
        }

        /// <summary>
        ///     Sets the values in the testresult object
        /// </summary>
        /// <returns></returns>
        public TestResult UpdateTestResult()
        {
            Position avgPosition = GetAveragePostion(this.PositionsList);

            CalculateDeviations(avgPosition);

            TestResult testResult = new TestResult(TestCase, Category);

            testResult.Datetime = this.startTime.ToLocalTime();
            testResult.TimeSpan = TimeSpan;
            testResult.Description = this.Description;

            testResult.TotalResults = this.PositionsList.Count();
            testResult.ZeroCount = this.ZeroCount;

            testResult.realPostion = this.MyPosition;
            testResult.avgPostion = avgPosition;


            testResult.Results = this.PositionsList.ToArray();

            // Deviation X Y to avg
            testResult.Distance2d = this.Distance2dList;
            testResult.MedianDistance2d = GetMedian(Distance2dList);
            testResult.ModeDistance2d = GetMode(Distance2dList); ;
            testResult.AverageDistance2d = GetAverage(Distance2dList);
            testResult.StandardDeviationDistance2d = GetStandardDeviation(Distance2dList);

            // Deviation X Y Z to avg
            testResult.Distance3d = this.Distance3dList;
            testResult.MedianDistance3d = GetMedian(Distance3dList);
            testResult.ModeDistance3d = GetMode(Distance3dList); ;
            testResult.AverageDistance3d = GetAverage(Distance3dList);
            testResult.StandardDeviationDistance3d = GetStandardDeviation(Distance3dList);

            // Deviation X Y to real
            testResult.Deviations2d = this.Deviations2d;
            testResult.Median2d = GetMedian(Deviations2d);
            testResult.Mode2d = GetMode(Deviations2d); ;
            testResult.Average2d = GetAverage(Deviations2d);
            testResult.StandardDeviation2d = GetStandardDeviation(Deviations2d);

            // Deviation X Y Zto real
            testResult.Deviations3d = this.Deviations3d;
            testResult.Median3d = GetMedian(Deviations3d);
            testResult.Mode3d = GetMode(Deviations3d); ;
            testResult.Average3d = GetAverage(Deviations3d);
            testResult.StandardDeviation3d = GetStandardDeviation(Deviations3d);
            return testResult;
        }
    }
}
