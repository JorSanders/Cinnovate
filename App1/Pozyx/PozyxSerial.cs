using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace App1.Pozyx
{
    /// <summary>
    /// Pozyx device reader
    /// </summary>
    public class PozyxSerial : IDisposable
    {
        #region Construction & destruction

        // serial port
        private CancellationTokenSource _Cancel;
        private Task _Worker;

        /// <summary>
        /// Construct device
        /// </summary>
        public PozyxSerial()
        {
            _Cancel = new CancellationTokenSource();
        }

        /// <summary>
        /// Destruct device
        /// </summary>
        ~PozyxSerial()
        {
            Dispose(false);
        }

        /// <summary>
        /// Close Device
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Cleanup Resources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _Cancel.Cancel();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the position
        /// </summary>
        public PozyxPositionEventArgs Position { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Signals position changed
        /// </summary>
        public event PozyxPositionEventHandler PositionChanged;

        /// <summary>
        /// Raises the position changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnPositionChanged(object sender, PozyxPositionEventArgs args)
        {
            PositionChanged?.Invoke(sender, args);
        }

        #endregion

        #region Open and worker

        /// <summary>
        /// Open device
        /// </summary>
        public void Open()
        {
            // starts worker
            var task = Worker(_Cancel.Token);
        }

        /// <summary>
        /// Actual worker
        /// </summary>
        private async Task Worker(CancellationToken cancel)
        {
            // keep trying
            while (!cancel.IsCancellationRequested)
            {
                // find arduino device
                var query = SerialDevice.GetDeviceSelector();
                var devices = await DeviceInformation.FindAllAsync(query);
                var items = devices.Select(d => new { Id = d.Id, Name = d.Name }).ToArray();
                var item = items.FirstOrDefault(i => i.Name.Contains("USB Serial Device") || i.Name.Contains("Arduino Mega"));

                // wait for device if not found
                if (item == null)
                {
                    // var watcher = DeviceInformation.CreateWatcher(query);
                    // var trigger = watcher.GetBackgroundTrigger(new DeviceWatcherEventKind[] { DeviceWatcherEventKind.Add, DeviceWatcherEventKind.Update });
                    // trigger

                    await Task.Delay(5000, cancel);
                    continue;
                }

                // open port
                using (var port = await SerialDevice.FromIdAsync(item.Id))
                {
                    // check for valid port
                    if (port == null) return;

                    // initialize port
                    port.BaudRate = 115200;
                    port.DataBits = 8;
                    port.StopBits = SerialStopBitCount.One;
                    port.Parity = SerialParity.None;
                    port.Handshake = SerialHandshake.None;
                    port.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                    port.WriteTimeout = TimeSpan.FromMilliseconds(1000);

                    // keep reading
                    await Reader(port, cancel);
                }
            }
        }

        /// <summary>
        /// Get end of line
        /// </summary>
        /// <returns></returns>
        private int GetEndOfLine(StringBuilder builder)
        {
            for (int i = 0; i < builder.Length; i++)
                if (builder[i] == 13) return i;
            return -1;
        }

        /// <summary>
        /// Parse Value
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private int? ParseValue(string source, string key)
        {
            // parse coordinates
            int index = source.IndexOf(key + "(mm): ");
            if (index == -1) return null;

            // get the term
            int term = source.IndexOf(',', index);
            if (term == -1) term = source.Length;

            // get sub string
            var text = source.Substring(index + 7, term - index - 7);

            // get values
            int value;
            if (!int.TryParse(text, out value)) return null;

            // return value
            return value;
        }

        /// <summary>
        /// Open reader
        /// </summary>
        /// <returns></returns>
        private async Task Reader(SerialDevice port, CancellationToken cancel)
        {
            // create reader
            var reader = new DataReader(port.InputStream);
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            reader.InputStreamOptions = InputStreamOptions.Partial;

            // create buffer
            var buffer = new StringBuilder();

            // keep reading
            while (!cancel.IsCancellationRequested)
            {
                try
                {
                    // create read task
                    var read = await reader.LoadAsync(20).AsTask(cancel);

                    // get bytes
                    var bytes = new byte[read];
                    reader.ReadBytes(bytes);

                    // get text
                    var text = Encoding.ASCII.GetString(bytes);
                    // var text = reader.ReadString(read);
                    buffer.Append(text);

                    // find latest position
                    PozyxPositionEventArgs position = null;

                    // keep reading results
                    while (buffer.Length > 0)
                    {
                        // keep processing
                        int i = 0;
                        for (; i < buffer.Length; i++)
                        {
                            if (buffer[i] == 10) continue;
                            if (buffer[i] == 13) break;
                        }

                        // done processing
                        if (i == buffer.Length)
                            break;

                        // remove from buffer
                        var line = buffer.ToString(0, i).Trim('\n', '\r');
                        buffer.Remove(0, i + 1);

                        // write line
                        Debug.WriteLine(line);

                        // POS,0x0,11005,12137,1767
                        var items = line.Split(',');
                        if (items.Length != 5) break;
                        if (items[0] != "POS") break;

                        // parse X, Y, Z
                        int x, y, z;
                        if (!int.TryParse(items[2], out x)) break;
                        if (!int.TryParse(items[3], out y)) break;
                        if (!int.TryParse(items[4], out z)) break;

                        // set new position
                        position = new PozyxPositionEventArgs(x, y, z);
                    }

                    // process if position received
                    if (position != null)
                    {
                        // store new position
                        Position = position;

                        // signal position changed
                        OnPositionChanged(this, position);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    break;
                }

            }
        }

        #endregion
    }
}
