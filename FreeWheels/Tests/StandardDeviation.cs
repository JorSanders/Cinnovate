﻿using FreeWheels.Classes;
using FreeWheels.Classes.PozyxApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FreeWheels.Tests
{
    public class StandardDeviation
    {

        private Pozyx _Pozyx;
        private object textBlock;

        public StandardDeviation(Pozyx pozyx)
        {
            _Pozyx = pozyx;
        }


        public async void ExportData(List<string> Export)
        {
            //  var Export = new List<string>() { "1; 2; 6", "x;y;z" };

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile sample = await folder.CreateFileAsync("dataExport.csv", CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteLinesAsync(sample, Export);
            string text = await FileIO.ReadTextAsync(sample);

        }


        public async Task<List<int>> coords()
        {

            var Export = new List<string>();
            Export.Add("Location X; Location Y; Location Z");

            _Pozyx.LetsGo();
            await Task.Delay(500);

            //sets default value
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
            int runTimeInMins = 2;
            int numResults = 0;
            DateTime dt = DateTime.Now.AddMinutes(runTimeInMins);
            int numberOfExpectedResults = (((((runTimeInMins * 60) * 1000) / timeDelay) / 100) * 83);

            //start sign
            Debug.WriteLine("------------------------");
            Debug.WriteLine("Calculating positions...");
            await Task.Delay(1000);

            //Expected Results

            while (dt > DateTime.Now)
            {
                //adds x y and z to lists
                xList.Add(PositioningData.PosX());
                yList.Add(PositioningData.PosY());
                zList.Add(PositioningData.PosZ());
                //calculates the sum of x y and z
                sumX += PositioningData.PosX();
                sumY += PositioningData.PosY();
                sumZ += PositioningData.PosZ();
                numResults++;

                if (numberOfExpectedResults >= 0)
                {
                    Debug.WriteLine("Expected results left " + numberOfExpectedResults + "...");
                    numberOfExpectedResults--;
                }
                if (numberOfExpectedResults == -1)
                {
                    Debug.WriteLine("Adding positioning finished...");
                    await Task.Delay(1000);

                    Debug.WriteLine("Loading results...");
                    await Task.Delay(1000);
                }

                await Task.Delay(timeDelay);
            }

            //prints the x y and z results
            Debug.WriteLine("number of results " + numResults);
            await Task.Delay(1000);
            for (int i = 0; i < numResults; i++)
            {
                Export.Add(xList[i] + ";" + yList[i] + ";" + zList[i]);

                //adds to list
                Debug.Write(xList[i] + " ");
                Debug.Write(yList[i] + " ");
                Debug.WriteLine(zList[i]);
            }
            await Task.Delay(1000);
            Debug.WriteLine("Finished adding results to list");
            await Task.Delay(1000);


            //calculates the average x y and z results
            averageX = sumX / numResults;
            averageY = sumY / numResults;
            averageZ = sumZ / numResults;

            float averageXConverted = averageX / convValue;
            float averageYConverted = averageY / convValue;
            float averageZConverted = averageZ / convValue;

            //Adds average to csv file
            Export.Add("Average result of " + numResults + " x,y,z positions: ;");
            Export.Add("Location X; Location Y; Location Z");
            Export.Add(averageXConverted + "(" + convType + ")" + ";" + averageYConverted + "(" + convType + ")" + ";" + averageZConverted + "(" + convType + ")");
            Debug.WriteLine("Finished adding average results");

            await Task.Delay(1000);
            Debug.WriteLine("----------------------------------------------");
            Debug.WriteLine("Average result of " + numResults + " x,y,z positions ");
            Debug.Write("Location ");
            Debug.Write("x(" + convType + "): ");
            Debug.Write(" y(" + convType + "): ");
            Debug.Write(averageY / convValue);
            Debug.Write(" z(" + convType + "): ");
            Debug.WriteLine(averageZ / convValue);
            Debug.WriteLine("----------------------------------------------");

            ExportData(Export);
            await Task.Delay(1000);
            Debug.WriteLine("Finished");
            return xList;
        }
        
    }
}
