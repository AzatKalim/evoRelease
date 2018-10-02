using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Evo20
{
    /// <summary>
    /// Статический класс, записываюжий логи
    /// </summary>
    public static class Log
    {
        //Имя файла=День_месяц_год_время
        static string file_name = DateTime.Now.Day.ToString()
            + DateTime.Now.Month.ToString()
            + DateTime.Now.Year.ToString() + "_"
            + DateTime.Now.Hour.ToString()
            + DateTime.Now.Minute.ToString()
            + DateTime.Now.Second.ToString()+ "log.txt";

        /// <summary>
        /// Запись сообщения в лог файл
        /// </summary>
        /// <param name="message"> сообщение</param>
        public static void WriteLog(string message)
        {
            lock (file_name)
            {
                File.AppendAllText(file_name, message + Environment.NewLine);
            }
        }
    }
}
