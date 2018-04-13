using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using PacketsLib;

namespace Evo_20form
{
    public enum SensorType
    {
        noDevice,
        DLY,
        DYS
    }
    class SensorData : Data
    {
        #region Params 

        public List<PacketsData> allPackets= new List<PacketsData>();

        public bool collectAllPackets = true;

        public PacketsCollection[] calibrationDLYPacketsCollection
        {
            private set;
            get;
        }

        public PacketsCollection[] checkDLYPacketsCollection
        {
            private set;
            get;
        }

        public PacketsCollection[] calibrationDYSPacketsCollection
        {
            private set;
            get;
        }

        public PacketsCollection[] checkDYSPacketsCollection
        {
            private set;
            get;
        }

        int calibrationDLYMaxPacketsCount;

        int calibrationDYSMaxPacketsCount;

        int checkDLYMaxPacketsCount;

        int checkDYSMaxPacketsCount;
      
        public ManualResetEvent packetsCollectedEvent;

        public SensorType sensorType;

        #endregion 

        public SensorData()
        {          
            packetsCollectedEvent = new ManualResetEvent(false);
        }

        public void InitialDictionaries(int[] colibratinTemperatures,
            int[] checkTemperatures ,
            int countOfPositionsDLY,
            int countOfPositionsDYS)
        {
            calibrationDLYPacketsCollection = new  PacketsCollection[colibratinTemperatures.Length];
            checkDLYPacketsCollection = new PacketsCollection[checkTemperatures.Length];
            calibrationDYSPacketsCollection = new PacketsCollection[colibratinTemperatures.Length];
            checkDYSPacketsCollection = new PacketsCollection[checkTemperatures.Length];
            for (int i = 0; i < colibratinTemperatures.Length; i++)
            {
                calibrationDLYPacketsCollection[i] = new PacketsCollection(colibratinTemperatures[i], countOfPositionsDLY, calibrationDLYMaxPacketsCount);
                calibrationDYSPacketsCollection[i] = new PacketsCollection(colibratinTemperatures[i], countOfPositionsDYS, calibrationDYSMaxPacketsCount);
            }
            for (int i = 0; i < checkTemperatures.Length; i++)
            {
                checkDLYPacketsCollection[i] = new PacketsCollection(checkTemperatures[i], countOfPositionsDLY, checkDLYMaxPacketsCount);
                checkDYSPacketsCollection[i] = new PacketsCollection(checkTemperatures[i], countOfPositionsDYS, checkDYSMaxPacketsCount);
            }
        }
       
        public bool AddCalibrationPacketData(PacketsData data,int temperature, int numberOfPosition)
        {
            if (collectAllPackets)
            {
                allPackets.Add(data);
            }

            if (sensorType == SensorType.DLY)
            {
                int index= FindTemperatureIndex(calibrationDLYPacketsCollection,temperature);
                if(index==-1)
                {
                    return false;
                }
                bool isSuccess=calibrationDLYPacketsCollection[index].AddPacketData(numberOfPosition,data);
                if(!isSuccess)
                    packetsCollectedEvent.Set();
                else
                    return true;
            }
            else
            {
                if (sensorType == SensorType.DYS)
                {
                    int index = FindTemperatureIndex(calibrationDYSPacketsCollection, temperature);
                    if (index == -1)
                    {
                        return false;
                    }
                    bool isSuccess = calibrationDYSPacketsCollection[index].AddPacketData(numberOfPosition, data);
                    if (!isSuccess)
                        packetsCollectedEvent.Set();
                    else
                        return true;                  
                }
            }
            return true;             
        }

        public bool AddCheckPacketData(PacketsData data, int temperature, int numberOfPosition)
        {
            if (collectAllPackets)
            {
                allPackets.Add(data);
            }
            if (sensorType == SensorType.DLY)
            {
                if (sensorType == SensorType.DLY)
                {
                    int index = FindTemperatureIndex(checkDLYPacketsCollection, temperature);
                    if (index == -1)
                    {
                        return false;
                    }
                    bool isSuccess = checkDLYPacketsCollection[index].AddPacketData(numberOfPosition, data);
                    if (!isSuccess)
                        packetsCollectedEvent.Set();
                    else
                        return true;
                }
            }
            else
            {
                if (sensorType == SensorType.DYS)
                {
                    if (sensorType == SensorType.DLY)
                    {
                        int index = FindTemperatureIndex(checkDYSPacketsCollection, temperature);
                        if (index == -1)
                        {
                            return false;
                        }
                        bool isSuccess = checkDYSPacketsCollection[index].AddPacketData(numberOfPosition, data);
                        if (!isSuccess)
                            packetsCollectedEvent.Set();
                        else
                            return true;
                    }
                }
            }
            return false;
        }

        public List<double> СalculateCalibrationAverage(int temperature, int numberOfPosition)
        {
            List<double> result = new List<double>();
            if (sensorType == SensorType.noDevice)
            {
                return null;
            }          
            if (sensorType == SensorType.DLY)
            {
                int index = FindTemperatureIndex(calibrationDLYPacketsCollection, temperature);
                if (index == -1)
                {
                    return null;
                }
                lock(calibrationDLYPacketsCollection)
                {
                   result=calibrationDLYPacketsCollection[index].MeanParams(numberOfPosition);
                   return result;
                }      
            }
            if (sensorType == SensorType.DYS)
            {
                int index = FindTemperatureIndex(calibrationDYSPacketsCollection, temperature);
                if (index == -1)
                {
                    return null;
                }
                lock(calibrationDYSPacketsCollection)
                {
                    result = calibrationDYSPacketsCollection[index].MeanParams(numberOfPosition);
                    return result;
                }      
            }
            return null;
        }

        public List<double> СalculateCheckAverage(int temperature, int numberOfPosition)
        {
            List<double> result = new List<double>();
            if (sensorType == SensorType.noDevice)
            {
                return null;
            }
            if (sensorType == SensorType.DLY)
            {
                int index = FindTemperatureIndex(checkDLYPacketsCollection, temperature);
                if (index == -1)
                {
                    return null;
                }
                lock (calibrationDLYPacketsCollection)
                {
                    result = checkDLYPacketsCollection[index].MeanParams(numberOfPosition);
                    return result;
                }
            }
            if (sensorType == SensorType.DYS)
            {
                int index = FindTemperatureIndex(checkDYSPacketsCollection, temperature);
                if (index == -1)
                {
                    return null;
                }
                lock (calibrationDYSPacketsCollection)
                {
                    result = checkDYSPacketsCollection[index].MeanParams(numberOfPosition);
                    return result;
                }
            }
            return null;
        }

        #region File work

        public override bool ReadSettings(ref StreamReader file)
        {
            bool isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДЛУ в режиме калибровка",
                ref  calibrationDLYMaxPacketsCount);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДУС в режиме калибровка",
                ref  calibrationDYSMaxPacketsCount);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДЛУ в режиме проверка",
                ref  checkDLYMaxPacketsCount);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДУС в режиме проверка",
                ref  checkDYSMaxPacketsCount);
            if (!isSuccess)
                return false;
            return true;
        }

        public void WritePackets(StreamWriter file, PacketsCollection[] data)
        {
            file.WriteLine(data.Length);

            for (int j = 0; j < data.Length; j++)
            {
                file.WriteLine(data[j].ToString());                
            }
        }

        public bool WriteAllPackets(StreamWriter file)
        {
            if (calibrationDLYPacketsCollection==null || calibrationDLYPacketsCollection.Length == 0)
            {
                file.WriteLine("Нет пакетов по калибровке ДЛУ");
            }
            else
            {
                WritePackets(file, calibrationDLYPacketsCollection);
            }
            if (calibrationDYSPacketsCollection==null || calibrationDYSPacketsCollection.Length == 0)
            {
                file.WriteLine("Нет пакетов по калибровке ДУС");
            }
            else
            {
                WritePackets(file, calibrationDYSPacketsCollection);
            }
            if (checkDLYPacketsCollection==null || checkDLYPacketsCollection.Length == 0)
            {
                file.WriteLine("Нет пакетов по Проверка ДЛУ");
            }
            else
            {
                WritePackets(file, checkDLYPacketsCollection);
            }
            if (checkDYSPacketsCollection==null || checkDYSPacketsCollection.Length == 0)
            {
                file.WriteLine("Нет пакетов по Проверка ДУС");
            }
            else
            {
                WritePackets(file, checkDYSPacketsCollection);
            }
            return true;
        }

        public bool WriteAllPackets()
        {
            StreamWriter file = new StreamWriter("allpackets.txt");
            return WriteAllPackets(file);
        }

        private bool ReadDataFromFile(StreamReader file,PacketsCollection[] collection)
        {
            int temperaturesCount = Convert.ToInt32(file.ReadLine());
            collection = new PacketsCollection[temperaturesCount];
            for (int i = 0; i < temperaturesCount; i++)
            {
                try
                {
                    collection[i] = new PacketsCollection(file);
                }
                catch (Exception exception)
                {
                    Log.WriteLog("Ошибка: Чтение файла данных пакетов при температуре" + i + " " + exception);
                    return false;
                }
            }
            return true;

        }

        public bool ReadDataFromFile(StreamReader file)
        {
            if (!ReadDataFromFile(file, calibrationDLYPacketsCollection))
            {
                return false;
            }
            if(!ReadDataFromFile(file,calibrationDYSPacketsCollection))
            {
                return false;
            }
            if (!ReadDataFromFile(file, checkDLYPacketsCollection))
            {
                return false;
            }
            if (!ReadDataFromFile(file, checkDYSPacketsCollection))
            {
                return false;
            }
            return true;
        }

        #endregion

        #region  Secondary functions

        public int PacketsArivedCountCalibration(int temperature,int numberOfPosition)
        {
            if (sensorType == SensorType.DLY)
            {
                int index = FindTemperatureIndex(calibrationDLYPacketsCollection, temperature);
                if (index == -1)
                {
                    return 0;
                }
                return calibrationDLYPacketsCollection[index][numberOfPosition].Count;
            }
            if (sensorType == SensorType.DYS)
            {

                int index = FindTemperatureIndex(calibrationDLYPacketsCollection, temperature);
                if (index == -1)
                {
                    return 0;
                }
                return calibrationDYSPacketsCollection[index][numberOfPosition].Count;
            }
            return 0;
        }

        public int PacketsArivedCountCheck(int temperature, int numberOfPosition)
        {
            if (sensorType == SensorType.DLY)
            {
                int index = FindTemperatureIndex(checkDLYPacketsCollection, temperature);
                if (index == -1)
                {
                    return 0;
                }
                return checkDLYPacketsCollection[temperature][numberOfPosition].Count;
            }
            if (sensorType == SensorType.DYS)
            {

                int index = FindTemperatureIndex(checkDLYPacketsCollection, temperature);
                if (index == -1)
                {
                    return 0;
                }
                return checkDYSPacketsCollection[temperature][numberOfPosition].Count;
            }
            return 0;
        }

        private int FindTemperatureIndex(PacketsCollection[] array, int temperature)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Temperature == temperature)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion 
    }

}
