using FreeWheels.Classes;
using FreeWheels.Classes.PozyxApi;
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
    public class StandardDeviation
    {

        private Pozyx _Pozyx;
        private Position position;
        public List<Position> PositionsList;
        public double[] DeviationsList;
        public int ZeroCount;
        List<string> Export = new List<string>();

        public StandardDeviation(Pozyx pozyx)
        {
            _Pozyx = pozyx;
            this.position = new Position();
        }
        public async void ExportData(List<string> Export)
        {
            //  var Export = new List<string>() { "1; 2; 6", "x;y;z" };

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile sample = await folder.CreateFileAsync("dataExport.csv", CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteLinesAsync(sample, Export);
            string text = await FileIO.ReadTextAsync(sample);

        }


        // public async Task<List<int>> coords()
        public async Task coords(int Duration, int Interval)
        {
          
            Export.Add("Location X; Location Y; Location Z");

            RegisterFunctions.ResetSys();
            await Task.Delay(2000);
            RegisterFunctions.FlashReset();
            await Task.Delay(2000);
            _Pozyx.LetsGo();
            await Task.Delay(2000);

            DateTime dt = DateTime.Now.AddMilliseconds(Duration);

            List<Position> PosList = new List<Position>();
            this.ZeroCount = 0;

            while (dt > DateTime.Now)
            {
                int x = PositioningData.PosX();
                int y = PositioningData.PosY();
                int z = PositioningData.PosZ();



                if(x == 0 && y == 0 && z == 0)
                {
                    this.ZeroCount += 1;
                }

                PosList.Add(new Position(x, y, z));
                Export.Add(x + ";" + y + ";" + z);
                await Task.Delay(Interval);
            }

            this.PositionsList = PosList;
            this.DeviationsList = GetDeviations(PosList);
        }

        public double[] GetDeviations(List<Position> data)
        {
            double[] deviations = new double[data.Count];

            for (int i = 0; i < data.Count; i ++)
            {
                // D² = A² + B² + C²
                deviations[i] = Math.Sqrt(Math.Pow(data[i].X, 2) + Math.Pow(data[i].Y, 2) + Math.Pow(data[i].Z, 2));
            }

            return deviations;
        }

        public double GetStandardDeviation()
        {
            //Convert Positions List
            double[] data = this.DeviationsList;

            int size = data.Length;
            double total = data.Sum();
            double average = total / size;

            double[] deviations = new double[size];

            // Calc Deviation on Average
            for(int i = 0; i < size; i++)
            {
                deviations[i] = Math.Pow(data[i] - average, 2);
            }

            double deviationsTotal = deviations.Sum();
            double averageDeviation = deviationsTotal / size;
            double standardDeviation = Math.Sqrt(averageDeviation);

            return standardDeviation;

        }

        public double GetAverage()
        {
            return this.DeviationsList.Sum() / this.DeviationsList.Length;
        }

        public double GetMedian()
        {
            double[] deviations = this.DeviationsList;
            Array.Sort(deviations);

            if(deviations.Length % 2 == 0)
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

            foreach(double deviation in this.DeviationsList)
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

            foreach(double key in counts.Keys)
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
            double median = GetMedian();
            string mode = GetMode();
            double average = GetAverage();
            double standardDeviation = GetStandardDeviation();

            TestResult testResult = new TestResult("TestCase", "Category");

            testResult.TotalResults = this.DeviationsList.Count();
            testResult.ZeroCount = this.ZeroCount;
            testResult.TimeSpan = "200";

            testResult.Median = median;
            testResult.Mode = mode;
            testResult.Average = average;
            testResult.StandardDeviation = standardDeviation;

            Export.Add("Average result of " + testResult.TotalResults + " x,y,z positions: ;");
            Export.Add("Location X; Location Y; Location Z");
            Export.Add(testResult.Average.ToString());
            ExportData(Export);
            
            return testResult;
        }

    }
}
