using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System;
using System.Windows.Forms;

namespace Evo20
{
    public class Log
    {
        public static Log Instance
        {
            get
            {
                if (instance == null)
                {
                    string pathToLogDir = Path.Combine(Directory.GetCurrentDirectory(), "Evo20.Log");
                    instance = new Log("Evo20", pathToLogDir);
                    instance.Start();
                }
                return instance;
            }
        }
        private static Log instance;

        private string pathToLogDirectory;
        private Queue<LogEntry> queue = new Queue<LogEntry>();
        private object syncQueue = new object();
        private AutoResetEvent newRecords = new AutoResetEvent(false);
        private AutoResetEvent stopped = new AutoResetEvent(false);

        public string Source { get; private set; }

        public bool Started { get; private set; }

        public string PathToLogFile
        {
            get { return Path.Combine(pathToLogDirectory, string.Format("{0}_{1}.log", Source, DateTime.Now.ToString("yyyyMMdd"))); }
        }

        public Log(string source, string pathToDir)
        {
            Source = source;
            pathToLogDirectory = pathToDir;
            if (!Directory.Exists(pathToLogDirectory))
                Directory.CreateDirectory(pathToLogDirectory);
        }

        public void Start()
        {
            Started = true;
            Thread thread = new Thread(WriteToFile);
            thread.Name = "Log writer";
            thread.IsBackground = true;
            thread.Start();
            newRecords.Set();
        }

        public void Stop()
        {
            if (Started)
            {
                Started = false;
                newRecords.Set();
                stopped.WaitOne();
            }
        }

        public void Write(System.Diagnostics.EventLogEntryType type, string message)
        {
            lock (syncQueue)
            {
                queue.Enqueue(new LogEntry()
                {
                    Time = DateTime.Now,
                    Type = type,
                    Message = message
                });
                newRecords.Set();
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
            System.Windows.Forms.MessageBox.Show(ex.Message + "\r\nSee logs for details.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        public void Exception(string message, Exception ex)
        {
            Write(System.Diagnostics.EventLogEntryType.Error, GetExceptionText(ex));
            System.Windows.Forms.MessageBox.Show(message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        public void Warning(string message)
        {
            Write(System.Diagnostics.EventLogEntryType.Warning, message);
        }

        public void Write(System.Diagnostics.EventLogEntryType type, string format, params object[] values)
        {
            Write(type, string.Format(format, values));
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
            try
            {
                int day = DateTime.Now.Day;
                var writer = new StreamWriter(PathToLogFile, true, Encoding.UTF8);
                do
                {
                    newRecords.WaitOne();
                    LogEntry[] entries;
                    lock (syncQueue)
                    {
                        entries = queue.ToArray();
                        queue.Clear();
                    }
                    foreach (var entry in entries)
                    {
                        if (entry.Time.Day != day)
                        {
                            day = entry.Time.Day;
                            writer.Close();
                            writer = new StreamWriter(PathToLogFile, true, Encoding.UTF8);
                        }
                        string line = string.Format("{0}|{1, 12}> {2}",
                            entry.Time.ToString("HH:mm:ss"),
                            entry.Type == 0 ? "Debug" : entry.Type.ToString(),
                            entry.Message);
                        writer.WriteLine(line);                 
                    }

                    writer.Flush();
                }
                while (Started);

                writer.Close();
                stopped.Set();
            }
            catch (Exception)
            {                
                throw; //Crash application
            }
        }

        private string GetExceptionText(Exception ex)
        {
            if (ex.InnerException == null)
                return ex.ToString();
            else
                return ex.ToString() + "\r\nInnerException:\r\n" + GetExceptionText(ex.InnerException);
        }

        private struct LogEntry
        {
            public DateTime Time;
            public System.Diagnostics.EventLogEntryType Type;
            public string Message;
        }
    }
}
    /// <summary>
    /// Статический класс, записываюжий логи
    /// </summary>
    //public static class Log
    //{
    //    //Имя файла=День_месяц_год_время
    //    static string file_name = DateTime.Now.Day.ToString()
    //        + DateTime.Now.Month.ToString()
    //        + DateTime.Now.Year.ToString() + "_"
    //        + DateTime.Now.Hour.ToString()
    //        + DateTime.Now.Minute.ToString()
    //        + DateTime.Now.Second.ToString()+ "log.txt";

    //    /// <summary>
    //    /// Запись сообщения в лог файл
    //    /// </summary>
    //    /// <param name="message"> сообщение</param>
    //    public static void WriteLog(string message)
    //    {
    //        lock (file_name)
    //        {
    //            File.AppendAllText(file_name, message + Environment.NewLine);
    //        }
    //    }
    //}

     //var allocationThread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
     //       {
     //           try
     //           {
     //               lock (controller)
     //                   terminal = controller.AllocateTerminal();
     //           }
     //           catch (Exception ex)
     //           {
     //               log.Error(string.Format("Allocate terminal error: ", ex.Message));
     //           }
     //       }))
     //       { IsBackground = true };

     //       allocationThread.Start();
     //       if (!allocationThread.Join(new TimeSpan(0, 0, 0, 0, AllocationTimeout)))
     //       {
     //           allocationThread.Abort();
     //           log.Error("Allocation terminal timeout");
     //           terminalID= -1;
     //       }
//}
