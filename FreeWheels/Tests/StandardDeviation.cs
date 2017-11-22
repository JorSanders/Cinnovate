using FreeWheels.Classes;
using FreeWheels.Classes.PozyxApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace FreeWheels.Tests
{
    public class StandardDeviation
    {

        private Pozyx _Pozyx;



        public StandardDeviation(Pozyx pozyx)
        {
            _Pozyx = pozyx;
        }

        public async Task<List<int>> coords()
        {


            _Pozyx.LetsGo();
            await Task.Delay(500);

            //Count number of bad data returns
            long xlowest = 10000;
            long countwrong = 0;

            //Interval xyz results
            List<int> xList = new List<int>();
            List<int> yList = new List<int>();
            List<int> zList = new List<int>();
            float sumX = 0;
            float sumY = 0;
            float sumZ = 0;
            float averageX = 0;
            float averageY = 0;
            float averageZ = 0;

            //Output 
            String convType = "cm";     // mm, cm, m
            int convValue = 10;         // mm = 1, cm = 10, m = 1000

            //Mean Deviation
            float distanceFromMeanX;

            //Time
            int timeDelay = 200;
            int numResults = 100;
            int numOfResults = 0;


            DateTime dt = DateTime.Now.AddMilliseconds(5000);




            Debug.WriteLine("------");
            Debug.WriteLine("------");


            //Device info


            //DispatcherTimer dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Tick += dispatcherTimer_Tick;
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);

            //dispatcherTimer.Start();


            //if (zList.Count == 10)
            //{
            //    dispatcherTimer.Stop();
            //}

            for (int i = 0; i < numResults; i++)
            {
                //
                if (dt < DateTime.Now)
                {
                    i = numResults;
                    Debug.WriteLine(numOfResults);
                }
                
                //
                xList.Add(PositioningData.PosX());
                yList.Add(PositioningData.PosY());
                zList.Add(PositioningData.PosZ());
                Debug.Write(xList[i] + " ");
                Debug.Write(yList[i] + " ");
                Debug.WriteLine(zList[i]);
                sumX += xList[i];
                sumY += yList[i];
                sumZ += zList[i];


                await Task.Delay(timeDelay);
                numOfResults++;


            }


            //average
            averageX = sumX / numResults;
            averageY = sumY / numResults;
            averageZ = sumZ / numResults;
            Debug.WriteLine("----------------------------------------------");
            Debug.WriteLine("Last " + numResults + " results averages: ");
            Debug.Write("Location ");
            Debug.Write("x(" + convType + "): ");
            Debug.Write(averageX / convValue);
            Debug.Write(" y(" + convType + "): ");
            Debug.Write(averageY / convValue);  
            Debug.Write(" z(" + convType + "): ");
            Debug.WriteLine(averageZ / convValue);
            Debug.WriteLine("----------------------------------------------");



            return xList;

        }

        //void dispatcherTimer_Tick(object sender, object e)
        //{


        //    xList.Add(PositioningData.PosX());
        //    yList.Add(PositioningData.PosY());
        //    zList.Add(PositioningData.PosZ());

        //    for (int i = 0; i < xList.Count; i++)
        //    {
        //        Debug.WriteLine(xList[i]);
        //    }


        //    String timeStamp = DateTime.Now.ToString();


        //}

    }
}
