using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Platform.ComponentModel;

namespace App1.Pozyx
{
    public class PozyxDevice
    {
        private static object _Mutex = new object();
        private static PozyxShield _Shield;
        private static PozyxSerial _Serial;

        /// <summary>
        /// Pozyx Device
        /// </summary>
        public PozyxDevice()
        {
            lock (_Mutex)
            {
                if (_Serial == null)
                {
                    _Serial = new PozyxSerial();
                    _Serial.PositionChanged += OnPositionChanged;
                    _Serial.Open();
                }
            }
        }

        /// <summary>
        /// Update position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPositionChanged(object sender, PozyxPositionEventArgs args)
        {
            Position = args;
            PositionChanged?.Invoke(this, args);
        }

        public PozyxPositionEventArgs Position { get; set; }

        /// <summary>
        /// Signal position changed
        /// </summary>
        public event PozyxPositionEventHandler PositionChanged;
    }
}
