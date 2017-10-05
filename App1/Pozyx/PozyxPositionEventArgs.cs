using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Pozyx
{
    /// <summary>
    /// Event Delegate for pozyx event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void PozyxPositionEventHandler(object sender, PozyxPositionEventArgs args);

    /// <summary>
    /// Construct pozyx event arguments
    /// </summary>
    public class PozyxPositionEventArgs : EventArgs
    {
        /// <summary>
        /// Construct event arguments
        /// </summary>
        /// <param name="date"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public PozyxPositionEventArgs(decimal x, decimal y, decimal z)
        {
            Date = DateTime.Now;
            X = (int)x;
            Y = (int)y;
            Z = (int)z;
        }

        /// <summary>
        /// Gets the date
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Gets the X (in mm)
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the Y (in mm)
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Gets the Z (in mm)
        /// </summary>
        public int Z { get; }
    }
}
