using System.Collections.Generic;
using System.Threading;
using Evo20.Packets;

namespace Evo20.Sensors
{
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
        
        Position[] CalibrationProfile
        {
            get;
        }

        Position[] CheckProfile
        {
            get;
        }

        List<PacketsCollection> CalibrationPacketsCollection
        {
            set;
            get;
        }

        List<PacketsCollection> CheckPacketsCollection
        {
            set;
            get;
        }

      
        bool AddCalibrationPacketData(PacketsData packetData,int temperatureOfCollect,int currentPositionNumber);
      
        bool AddCheckPacketData(PacketsData packetData,int temperatureOfCollect,int currentPositionNumber);

        List<double> СalculateCalibrationAverage(int temperature, int numberOfPosition);

        List<double> СalculateCheckAverage(int temperature, int numberOfPosition);

        int PacketsArivedCountCalibration(int temperature, int numberOfPosition);
   
        int PacketsArivedCountCheck(int temperature, int numberOfPosition);

        double[][,] GetCalibrationAdcCodes();

        double[][,] GetCheckAdcCodes();

        bool WriteRedPackets(string path);
    }
}
