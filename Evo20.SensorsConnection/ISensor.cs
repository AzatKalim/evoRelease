using System.Collections.Generic;
using System.Threading;
using Evo20.PacketsLib;
using Evo20.Log;

namespace Evo20.SensorsConnection
{
    public struct ProfilePart
    {
        public int axisX;
        public int axisY;
        public int speedX;
        public int speedY;
        public ProfilePart(int x, int y, int speedX, int speedY)
        {
            axisX = x;
            axisY = y;
            this.speedX = speedX;
            this.speedY = speedY;
        }
        public ProfilePart(int x, int y)
            : this(x, y, 0, 0){ }
    }


    /// <summary>
    /// Интерфейс для датчиков, чтобы с ними работала программа необходмо его реализовать( наследоваться от него)!!! 
    /// </summary>
    public interface ISensor
    {
        /// <summary>
        /// Название датчика
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Событие сбора необходимого числа пакетов
        /// </summary>
        AutoResetEvent PacketsCollectedEvent
        {
            set;
            get;
        }
        
        /// <summary>
        /// Должен возвращать профиль калибровки
        /// </summary>
        ProfilePart[] CalibrationProfile
        {
            get;
        }

        /// <summary>
        /// Должен возвращать профиль проверки
        /// </summary>
        ProfilePart[] CheckProfile
        {
            get;
        }

        /// <summary>
        /// Возвращает массив коллекций пакетов калибровки
        /// </summary>
        List<PacketsCollection> CalibrationPacketsCollection
        {
            set;
            get;
        }

        /// <summary>
        /// Возвращает массив коллекций пакетов проверки
        /// </summary>
        List<PacketsCollection> CheckPacketsCollection
        {
            set;
            get;
        }

        /// <summary>
        /// Дабавляет пакет дата(4 пакета ) к пакетам калибровки
        /// </summary>
        /// <param name="packetData"> 4 проверенных пакета </param>
        /// <param name="temperatureOfCollect"> температура сборки пакета</param>
        /// <param name="currentPositionNumber"> номер текущей позиции </param>
        /// <returns> true - пакет добален, false все пакеты собраны</returns>
        bool AddCalibrationPacketData(PacketsData packetData,int temperatureOfCollect,int currentPositionNumber);
        
        /// <summary>
        /// Дабавляет пакет дата(4 пакета ) к пакетам проверки
        /// </summary>
        /// <param name="packetData"> 4 проверенных пакета </param>
        /// <param name="temperatureOfCollect"> температура сборки пакета</param>
        /// <param name="currentPositionNumber"> номер текущей позиции </param>
        /// <returns> true - пакет добален, false все пакеты собраны</returns>
        bool AddCheckPacketData(PacketsData packetData,int temperatureOfCollect,int currentPositionNumber);

        /// <summary>
        /// Возвращает среднее значение по пакетам калибровки
        /// </summary>
        /// <param name="temperature"> температура </param>
        /// <param name="numberOfPosition"> номер позиции </param>
        /// <returns>список значений в пакетах </returns>
        List<double> СalculateCalibrationAverage(int temperature, int numberOfPosition);

        /// <summary>
        /// Возвращает среднее значение по пакетам проверки
        /// </summary>
        /// <param name="temperature"> температура </param>
        /// <param name="numberOfPosition"> номер позиции </param>
        /// <returns>список значений в пакетах </returns>
        List<double> СalculateCheckAverage(int temperature, int numberOfPosition);

        /// <summary>
        /// Число собраных пакетов калибровки
        /// </summary>
        /// <param name="temperature">температура</param>
        /// <param name="numberOfPosition">номер позиции</param>
        /// <returns>число пакетов </returns>
        int PacketsArivedCountCalibration(int temperature, int numberOfPosition);

        /// <summary>
        /// Число собраных пакетов проверки
        /// </summary>
        /// <param name="temperature">температура</param>
        /// <param name="numberOfPosition">номер позиции</param>
        /// <returns>число пакетов </returns>
        int PacketsArivedCountCheck(int temperature, int numberOfPosition);

        double[][,] GetCalibrationADCCodes();

        double[][,] GetCheckADCCodes();

        bool WriteRedPackets();
    }
}
