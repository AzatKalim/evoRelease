using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Evo20.Utils;
using Evo20.Sensors;

namespace Evo20.Math
{
    public static class CalculatorCoefficients
    {

        private const double Factor = 9.807;

        #region DLY coefficients

        private static double[][][] ComputeCalibrationCoefficentsDLY(ISensor dly)
        {
            Log.Instance.Info("Рассчет ДЛУ");
            var a = new double[dly.CalibrationPacketsCollection.Count][][];
            for (var i = 0; i < a.Length; i++)
            {
                a[i] = new double[dly.CalibrationPacketsCollection[i].PositionCount][];
                for (var j = 0; j < dly.CalibrationPacketsCollection[i].PositionCount; j++)
                {
                    a[i][j] = new double[4];
                    var meanA = dly.CalibrationPacketsCollection[i].MeanA(j);
                    a[i][j][0] = 1;
                    a[i][j][1] = meanA[0];
                    a[i][j][2] = meanA[1];
                    a[i][j][3] = meanA[2];
                }
            }      
            var b = DLYModelVectors(dly.CalibrationProfile);
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

        private static double[][][] ComputeCalibrationCoefficentsDYS(ISensor dys)
        {
            Log.Instance.Info("Рассчет ДУС");
            var a = ComputeAMatrix(dys);
            var b = GetModelDYS(dys.CalibrationProfile);
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

        private static double[][][] ComputeAMatrix(ISensor dys)
        {
            Log.Instance.Info("Вычисление матрицы А ДУС");

            var a = new double[dys.CalibrationPacketsCollection.Count][][];
            for (var i = 0; i < a.Length; i++)
            {
                a[i] = new double[dys.CalibrationPacketsCollection[i].PositionCount][];
                for (var j = 0; j < dys.CalibrationPacketsCollection[i].PositionCount; j++)
                {
                    a[i][j]= new double[13];
                    var w = dys.CalibrationPacketsCollection[i].MeanW(j);
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

            var coefficentsDly = ComputeCalibrationCoefficentsDLY(dly);
            var temperatureCoefficentsDly = ComputeTemperatureCalibrationCoefficents(dly, dly.CalibrationProfile.Length);

            var coefficentsDys = ComputeCalibrationCoefficentsDYS(dys);
            var temperatureCoefficentsDys = ComputeTemperatureCalibrationCoefficents(dys, dys.CalibrationProfile.Length);

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

        private static double[][] ComputeTemperatureCalibrationCoefficents(ISensor sensor,int posCount)
        {
            Log.Instance.Info("Рассчет векторов {0} по температурам", sensor.Name);
            var mean = new double[sensor.CalibrationPacketsCollection.Count][];
            for (var i = 0; i < mean.Length; i++)
                for (var j = 0; j < posCount; j++)
                    mean[i] = sensor.CalibrationPacketsCollection[i].MeanUa(j);
            return mean;
        }

        #region Secondary functions

        private static void WriteMatrix(double[][] matrix, ref StreamWriter file)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    string str = matrix[i][j].ToString(CultureInfo.InvariantCulture);
                    file.Write(str);
                    if (str.Length < 20)
                        file.Write(new string(' ', 20 - str.Length));
                }

                file.Write(Environment.NewLine);
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
                        var buffer = new StringBuilder();
                        for (var k = 0; k < matrix[0][0].Length; k++)
                        {
                            string str = matrix[i][j][k].ToString(CultureInfo.InvariantCulture);
                            buffer.Append(str);
                            if (str.Length < 20)
                                buffer.Append(new string(' ', 20 - str.Length));
                            
                        }
                        file.WriteLine(buffer);
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
        #endregion
    }
}
