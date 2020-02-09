using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Evo20.Packets;
using Evo20.Utils;
using Evo20.Sensors;

namespace Evo20.Math
{
    public static class CalculatorCoefficients
    {

        private const double Factor = 9.807;

        #region DLY coefficients

        private static double[][][] ComputeCalibrationCoefficentsDLY(IList<PacketsCollection> packetsCollection,
            Position[] profile)
        {
            Log.Instance.Info("Рассчет ДЛУ");
            var a = new double[packetsCollection.Count][][];
            for (var i = 0; i < a.Length; i++)
            {
                a[i] = new double[packetsCollection[i].PositionCount][];
                for (var j = 0; j < packetsCollection[i].PositionCount; j++)
                {
                    a[i][j] = new double[4];
                    var meanA = packetsCollection[i].MeanA(j);
                    a[i][j][0] = 1;
                    a[i][j][1] = meanA[0];
                    a[i][j][2] = meanA[1];
                    a[i][j][3] = meanA[2];
                }
            }

            var b = DLYModelVectors(profile);
            return ComputeCalibrationCoefficents(a, b);
        }

        //Эталонных ускорения ДЛУ
        public static double[][] DLYModelVectors(Position[] profile)
        {
            Log.Instance.Info("Эталонные вектора ДЛУ");

            double[][] b = new double[profile.Length][];
            var dlyMatrix = ProfileToPositionArray(profile);
            for (int n = 0; n < profile.Length; n++)
            {
                b[n] = new double[3];
                b[n][0] = -Factor * System.Math.Sin(dlyMatrix[n][1] * System.Math.PI / 180) *
                          System.Math.Cos(dlyMatrix[n][0] * System.Math.PI / 180);
                b[n][1] = Factor * System.Math.Cos(dlyMatrix[n][1] * System.Math.PI / 180);
                b[n][2] = -Factor * System.Math.Sin(dlyMatrix[n][1] * System.Math.PI / 180) *
                          System.Math.Sin(dlyMatrix[n][0] * System.Math.PI / 180);
            }
            return b;
        }

        #endregion

        #region DYS coefficents

        private static double[][][] ComputeCalibrationCoefficentsDYS(List<PacketsCollection> packetsCollections, Position[] profiles)
        {
            Log.Instance.Info("Рассчет ДУС");
            var a = ComputeAMatrix(packetsCollections);
            var b = GetModelDYS(profiles);
            return ComputeCalibrationCoefficents(a, b);
        }

        private static double[][] GetModelDYS(Position[] dysProfile)
        {
            var result = new double[dysProfile.Length][];
            for (int i = 0; i < dysProfile.Length; i++)
            {
                result[i] = new double[3];
                result[i][0] = dysProfile[i].SecondPosition == 90 ? dysProfile[i].SpeedFirst : 0;
                result[i][1] = dysProfile[i].SpeedSecond;
                result[i][2] = dysProfile[i].SecondPosition == 90 ? 0 : dysProfile[i].SpeedFirst;
            }

            return result;
        }
        private static double[][][] ComputeCalibrationCoefficents(double[][][] a, double[][] b)
        {
            var result = new double[a.Length][][];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = OneCompute(a[i], b);
            }

            return result;
        }

        public static double[][] OneCompute(double[][] a, double[][] b)
        {
            var aTrans = a.Transpose();
            var step1 = Matrix.Multiply(aTrans, a);
            var step2 = step1.Inverse();
            var step3 = Matrix.Multiply(aTrans, b);
            return Matrix.Multiply(step2, step3);
        }

        private static double[][][] ComputeAMatrix(List<PacketsCollection> packetsCollections)
        {
            Log.Instance.Info("Вычисление матрицы А ДУС");

            var a = new double[packetsCollections.Count][][];
            for (var i = 0; i < a.Length; i++)
            {
                a[i] = new double[packetsCollections[i].PositionCount][];
                for (var j = 0; j < packetsCollections[i].PositionCount; j++)
                {
                    a[i][j]= new double[13];
                    var w = packetsCollections[i].MeanW(j);
                    a[i][j][0] = 1;
                    var counter = 0;
                    for (var k = 1; k < 5; k++)
                    {
                        a[i][j][++counter] = System.Math.Pow(w[0],k);
                        a[i][j][++counter] = System.Math.Pow(w[1],k);
                        a[i][j][++counter] = System.Math.Pow(w[2],k);
                    }                 
                }              
            }

            return a;
        }

        #endregion

        public static bool CalculateCoefficients(ISensor dly, ISensor dys, StreamWriter file)
        {
            Log.Instance.Info("CalculateCoefficients");

            var dlyPacketsCollections = UnionByTemperature(dly.CalibrationPacketsCollection);
            var coefficentsDly = ComputeCalibrationCoefficentsDLY(dlyPacketsCollections, dly.CalibrationProfile);
            Log.Instance.Info($"Рассчет векторов {dly.Name} по температурам");
            var temperatureCoefficentsDly =
                ComputeTemperatureCalibrationCoefficents(dlyPacketsCollections, dly.CalibrationProfile.Length);

            var dysPacketsCollections = UnionByTemperature(dys.CalibrationPacketsCollection);
            var coefficentsDys = ComputeCalibrationCoefficentsDYS(dysPacketsCollections, dys.CalibrationProfile);
            Log.Instance.Info($"Рассчет векторов {dys.Name} по температурам");
            var temperatureCoefficentsDys =
                ComputeTemperatureCalibrationCoefficents(dysPacketsCollections, dys.CalibrationProfile.Length);

            file.WriteLine("коэффициенты ДЛУ по ускорениям");
            WriteMatrix(coefficentsDly, ref file);
            file.WriteLine("коэффициенты ДУС по угловым скоростям");
            WriteMatrix(coefficentsDys, ref file);

            file.WriteLine("коэффициенты ДЛУ по температуре ");
            WriteMatrix(temperatureCoefficentsDly, ref file);
            file.WriteLine("коэффициенты ДУC по температуре ");
            WriteMatrix(temperatureCoefficentsDys, ref file);
            return true; 
        }

        private static double[][] ComputeTemperatureCalibrationCoefficents(List<PacketsCollection> packetsCollection, int positionCount)
        {
            var mean = new double[packetsCollection.Count][];
            for (var i = 0; i < mean.Length; i++)
                for (var j = 0; j < positionCount; j++)
                    mean[i] = packetsCollection[i].MeanUa(j);
            return mean;
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
                    file.WriteLine(i);
                    for (var j = 0; j < matrix[0].Length; j++)
                    {
                        for (var k = 0; k < matrix[0][0].Length; k++)
                        {
                            file.WriteLine(matrix[i][j][k].ToString(CultureInfo.InvariantCulture));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Exception(ex);
            }
        }

        private static int[][] ProfileToPositionArray(Position[] profile)
        {
            return profile.Select(position => new[] {(int) position.SecondPosition, (int) position.FirstPosition})
                .ToArray();
        }

        private static List<PacketsCollection> UnionByTemperature(List<PacketsCollection> packetsCollections)
        {
            if (packetsCollections == null)
                throw new ArgumentNullException(nameof(packetsCollections));
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
