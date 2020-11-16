using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Evo20.Math.Calculators;
using Evo20.Packets;
using Evo20.Sensors;
using Evo20.Utils;

namespace Evo20.Math
{
    public class CoefficentCalculator
    {
        private readonly BaseCalculator _dlyCalculator;
        private readonly BaseCalculator _dysCalculator;

        public CoefficentCalculator()
        {
            _dlyCalculator = new DLYCalculator();
            _dysCalculator = new DYSCalculator();
        }

        public bool CalculateCoefficients(ISensor dly, ISensor dys, StreamWriter file)
        {
            Log.Instance.Info("CalculateCoefficients");

            var dlyPacketsCollections = UnionByTemperature(dly.CalibrationPacketsCollection);
            var coefficentsDly = _dlyCalculator.ComputeCalibrationCoefficents(dlyPacketsCollections,
                dly.CalibrationProfile, Config.Instance.DLYComputeDegree);
            Log.Instance.Info($"Рассчет векторов {dly.Name} по температурам");
            var temperatureCoefficentsDly =
                _dlyCalculator.ComputeTemperatureCalibrationCoefficents(dlyPacketsCollections,
                    dly.CalibrationProfile.Length);

            var dysPacketsCollections = UnionByTemperature(dys.CalibrationPacketsCollection);
            var coefficentsDys = _dysCalculator.ComputeCalibrationCoefficents(dysPacketsCollections,
                dys.CalibrationProfile, Config.Instance.DYSComputeDegree);
            Log.Instance.Info($"Рассчет векторов {dys.Name} по температурам");
            var temperatureCoefficentsDys =
                _dysCalculator.ComputeTemperatureCalibrationCoefficents(dysPacketsCollections,
                    dys.CalibrationProfile.Length);

            WriteMatrix(coefficentsDly, ref file);
            WriteMatrix(coefficentsDys, ref file);
            WriteMatrix(temperatureCoefficentsDly, ref file);
            WriteMatrix(temperatureCoefficentsDys, ref file);
            return true;
        }

        #region Secondary functions

        private static void WriteMatrix(double[][] matrix, ref StreamWriter file)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    file.WriteLine(matrix[i][j].ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        private static void WriteMatrix(double[][][] matrix, ref StreamWriter file)
        {
            try
            {
                for (var i = 0; i < matrix.Length; i++)
                {
                    for (var j = 0; j < matrix[0][0].Length; j++)
                    {
                        for (var k = 0; k < matrix[0].Length; k++)
                        {
                            file.WriteLine(matrix[i][k][j].ToString(CultureInfo.InvariantCulture));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Exception(ex);
            }
        }

        private static List<PacketsCollection> UnionByTemperature(List<PacketsCollection> packetsCollections)
        {
            if (packetsCollections == null) throw new ArgumentNullException(nameof(packetsCollections));
            var temperaturesDictionary = new Dictionary<int, PacketsCollection>();
            foreach (var packetsCollection in packetsCollections)
            {
                if (!temperaturesDictionary.ContainsKey(packetsCollection.Temperature))
                {
                    temperaturesDictionary.Add(packetsCollection.Temperature, packetsCollection);
                }
                else
                {
                    var existedCollection = temperaturesDictionary[packetsCollection.Temperature];
                    for (int i = 0; i < packetsCollection.PositionCount; i++)
                    {
                        for (int j = 0; j < packetsCollection.MeanA(i).Length; j++)
                        {
                            existedCollection.MeanA(i)[j] =
                                (existedCollection.MeanA(i)[j] + packetsCollection.MeanA(i)[j]) / 2;
                            existedCollection.MeanW(i)[j] =
                                (existedCollection.MeanW(i)[j] + packetsCollection.MeanW(i)[j]) / 2;
                            existedCollection.MeanUa(i)[j] =
                                (existedCollection.MeanUa(i)[j] + packetsCollection.MeanUa(i)[j]) / 2;
                            existedCollection.MeanUw(i)[j] =
                                (existedCollection.MeanUw(i)[j] + packetsCollection.MeanUw(i)[j]) / 2;
                        }
                    }
                }
            }

            return temperaturesDictionary.Values.ToList();
        }

        #endregion
    }
}
