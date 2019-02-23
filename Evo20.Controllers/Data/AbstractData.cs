using System;
using System.IO;
using System.Linq;
using Evo20.Utils;

namespace Evo20.Controllers.Data
{
    public abstract class AbstractData
    {
        public abstract bool ReadSettings(ref  StreamReader file);
        
        public bool ReadParamFromFile(ref StreamReader file, string checkString, ref double param)
        {
            string templine = file.ReadLine();
            if (templine == null)
                throw new ArgumentNullException(nameof(templine));
            string[] temp = templine.Split(':');
            if (temp[0] != checkString)
            {
                Log.Instance.Error("Не верна строка файла настроек:{0}",checkString);
                return false;
            }
            else
            {
                try
                {
                    param = Convert.ToDouble(temp[1]);
                }
                catch (Exception)
                {
                    if (temp[1].Contains(','))
                        temp[1]=temp[1].Replace(',', '.');
                    else
                        temp[1]=temp[1].Replace('.', ',');
                    try
                    {
                        param = Convert.ToDouble(temp[1]);
                    }
                    catch(Exception ex)
                    {
                        Log.Instance.Error("Не верна строка файла настроек:{0}",checkString);
                        Log.Instance.Exception(ex);
                        return false;
                    }                 
                }
            }
            return true;
        }
   
        public bool ReadParamFromFile(ref StreamReader file, string checkString, ref int param)
        {
            string templine = file.ReadLine();
            if (templine != null)
            {
                string[] temp = templine.Split(':');
                if (temp[0] != checkString)
                {
                    Log.Instance.Error("Не верна строка файла настроек:" + checkString);
                    return false;
                }
                else
                {
                    try
                    {
                        param = Convert.ToInt32(temp[1]);
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error("Не верна строка файла настроек:{0}",checkString);
                        Log.Instance.Exception(ex);
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
