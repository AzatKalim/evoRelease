using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Evo20.Utils;

namespace Evo20.GUI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            WriteFileToLog("Evo20.GUI.exe.config");
            WriteFileToLog("settings.txt");
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += ThreadException;
                AppDomain.CurrentDomain.UnhandledException +=
                    CurrentDomain_UnhandledException;
                Application.Run(new MainForm());

            }
            catch (Exception exception)
            {
                Log.Instance.Exception(exception);
            }
        }
        private static void ThreadException(object sender, ThreadExceptionEventArgs t)
        {
            try
            {
                Log.Instance.Warning("Thread application error!");
                Log.Instance.Exception(t.Exception);
            }
            catch(Exception ex)
            {
                try
                {
                    Log.Instance.Warning("Fatal error");
                    Log.Instance.Exception(ex);
                }
                finally
                {
                    Application.Exit();
                }
            }
            Application.Exit();
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                Log.Instance.Warning("Critical domain error!");
                Log.Instance.Exception(ex);
            }
            catch (Exception exc)
            {
                try
                {
                    Log.Instance.Warning("Critical write error!");
                    Log.Instance.Exception(exc);
                }
                finally
                {
                    Application.Exit();
                }
            }
            Application.Exit();
        }

        private static void WriteFileToLog(string name)
        {
            using (var reader = new StreamReader(name))
                Log.Instance.Info(reader.ReadToEnd());
        }
    }
}
