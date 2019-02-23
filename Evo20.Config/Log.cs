using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Evo20.Utils
{
    public class Log : IDisposable
    {
        public static Log Instance
        {
            get
            {
                if (_instance == null)
                {
                    string pathToLogDir = Path.Combine(Directory.GetCurrentDirectory(), "Evo20.Log");
                    _instance = new Log("Evo20", pathToLogDir);
                    _instance.Start();
                }
                return _instance;
            }
        }
        private static Log _instance;

        private readonly string _pathToLogDirectory;
        private readonly Queue<LogEntry> _queue = new Queue<LogEntry>();
        private readonly object _syncQueue = new object();
        private readonly AutoResetEvent _newRecords = new AutoResetEvent(false);
        private readonly AutoResetEvent _stopped = new AutoResetEvent(false);

        public string Source { get; }

        public bool Started { get; private set; }

        public string PathToLogFile => Path.Combine(_pathToLogDirectory,
            $"{Source}_{DateTime.Now:yyyyMMdd}.log");

        public Log(string source, string pathToDir)
        {
            Source = source;
            _pathToLogDirectory = pathToDir;
            if (!Directory.Exists(_pathToLogDirectory))
                Directory.CreateDirectory(_pathToLogDirectory);
        }

        public void Start()
        {
            Started = true;
            Thread thread = new Thread(WriteToFile) {Name = "Log writer", IsBackground = true};
            thread.Start();
            _newRecords.Set();
        }

        public void Stop()
        {
            if (Started)
            {
                Started = false;
                _newRecords.Set();
                _stopped.WaitOne();
            }
        }

        public void Write(System.Diagnostics.EventLogEntryType type, string message)
        {
            lock (_syncQueue)
            {
                _queue.Enqueue(new LogEntry()
                {
                    Time = DateTime.Now,
                    Type = type,
                    Message = message
                });
                _newRecords.Set();
            }
        }


        public void Debug(string message)
        {
            Write(0, message);
        }

        public void Info(string message)
        {
            Write(System.Diagnostics.EventLogEntryType.Information, message);
        }

        public void Error(string message)
        {
            Write(System.Diagnostics.EventLogEntryType.Error, message);
            System.Windows.Forms.MessageBox.Show(message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        public void Exception(Exception ex)
        {
            Write(System.Diagnostics.EventLogEntryType.Error, GetExceptionText(ex));
            //System.Windows.Forms.MessageBox.Show(ex.Message + "\r\nSee logs for details.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        public void Exception(string message, Exception ex)
        {
            Write(System.Diagnostics.EventLogEntryType.Error, GetExceptionText(ex));
            //System.Windows.Forms.MessageBox.Show(message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        public void Warning(string message)
        {
            Write(System.Diagnostics.EventLogEntryType.Warning, message);
        }

        public void Debug(string format, params object[] values)
        {
            Debug(string.Format(format, values));
        }

        public void Info(string format, params object[] values)
        {
            Info(string.Format(format, values));
        }

        public void Error(string format, params object[] values)
        {
            Error(string.Format(format, values));
        }

        public void Warning(string format, params object[] values)
        {
            Warning(string.Format(format, values));
        }

        private void WriteToFile()
        {
            int day = DateTime.Now.Day;
            var writer = new StreamWriter(PathToLogFile, true, Encoding.UTF8);
            do
            {
                _newRecords.WaitOne();
                LogEntry[] entries;
                lock (_syncQueue)
                {
                    entries = _queue.ToArray();
                    _queue.Clear();
                }
                foreach (var entry in entries)
                {
                    if (entry.Time.Day != day)
                    {
                        day = entry.Time.Day;
                        writer.Close();
                        writer = new StreamWriter(PathToLogFile, true, Encoding.UTF8);
                    }
                    string line =
                        $"{entry.Time:HH:mm:ss}|{(entry.Type == 0 ? "Debug" : entry.Type.ToString()),12}> {entry.Message}";
                    writer.WriteLine(line);                 
                }

                writer.Flush();
            }
            while (Started);

            writer.Close();
            _stopped.Set();
        }

        private string GetExceptionText(Exception ex)
        {
            if (ex.InnerException == null)
                return ex.ToString();
            else
                return ex + "\r\nInnerException:\r\n" + GetExceptionText(ex.InnerException);
        }

    private struct LogEntry
        {
            public DateTime Time;
            public System.Diagnostics.EventLogEntryType Type;
            public string Message;
        }

        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _newRecords.Dispose();
                    _stopped.Dispose();
                }

                _disposedValue = true;
            }
        }   
        public void Dispose()
        {
            Dispose(true);         
        }
        #endregion
    }
}