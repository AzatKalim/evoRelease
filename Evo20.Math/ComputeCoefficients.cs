using System;
using System.IO;
using System.Linq;
using Evo20.Utils;
using Evo20.Sensors;

namespace Evo20.Math
{
    public static class CalculatorCoefficients
    {
        #region Constants

        private const int KCount = 72;
        private const int SCount = 3;
        private const int NdlyCount = 72;
        private const int RCount = 39;
        private const double Factor = 9.807;

        #region DYS 

        public static readonly double[][] DYS_MATRIX =
        {
            new double[] {0, 0, 0},
            new double[] {0, 2, 0},
            new double[] {0, 8, 0},
            new double[] {0, 16, 0},
            new double[] {0, 64, 0},
            new double[] {0, 112, 0},
            new double[] {0, 128, 0},
            new double[] {0, -2, 0},
            new double[] {0, -8, 0},
            new double[] {0, -16, 0},
            new double[] {0, -64, 0},
            new double[] {0, -112, 0},
            new double[] {0, -128, 0},
            new double[] {0, 0, 0},
            new double[] {0, 0, -2},
            new double[] {0, 0, -8},
            new double[] {0, 0, -16},
            new double[] {0, 0, -64},
            new double[] {0, 0, -112},
            new double[] {0, 0, -128},
            new double[] {0, 0, 2},
            new double[] {0, 0, 8},
            new double[] {0, 0, 16},
            new double[] {0, 0, 64},
            new double[] {0, 0, 112},
            new double[] {0, 0, 128},
            new double[] {0, 0, 0},
            new double[] {2, 0, 0},
            new double[] {8, 0, 0},
            new double[] {16, 0, 0},
            new double[] {64, 0, 0},
            new double[] {112, 0, 0},
            new double[] {128, 0, 0},
            new double[] {-2, 0, 0},
            new double[] {-8, 0, 0},
            new double[] {-16, 0, 0},
            new double[] {-64, 0, 0},
            new double[] {-112, 0, 0},
            new double[] {-128, 0, 0}
        };
        #endregion

        #endregion

        #region DLY coefficients

        private static double[][][] ComputeCalibrationCoefficentsDLY(ISensor dly)
        {
            Log.Instance.Info("Рассчет ДЛУ");
            var a = new double[8][][];
            for (var i = 0; i < 8; i++)
            {
                a[i] = new double[KCount][];
                for (var j = 0; j < KCount; j++)
                {
                    a[i][j] = new double[4];
                    var meanA = dly.CalibrationPacketsCollection[0].MeanA(j);
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

            double[][] b = new double[NdlyCount][];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = new double[SCount];
            }

            var dlyMatrix = ProfileToPositionArray(profile);
            for (int n = 0; n < NdlyCount; n++)
            {
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
            var b = DYS_MATRIX;
            return ComputeCalibrationCoefficents(a, b);
        }

        private static double[][][] ComputeCalibrationCoefficents(double[][][] a, double[][] b)
        {
            var result = new double[8][][];
            for (var i = 0; i < 8; i++)
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

            var a = new double[8][][];
            for (var i = 0; i < a.Length; i++)
                a[i] = new double[RCount][];

            for (var i = 0; i < a.Length; i++)
            {
                for (var j = 0; j < RCount;j++)
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
            var temperatureCoefficentsDly = ComputeTemperatureCalibrationCoefficents(dly,KCount);

            var coefficentsDys = ComputeCalibrationCoefficentsDYS(dys);
            var temperatureCoefficentsDys = ComputeTemperatureCalibrationCoefficents(dys,RCount);

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
            Log.Instance.Info("Рассчет векторов {0} по температурам",sensor.Name);
            var mean = new double[8][];
            for (var i = 0; i < 8; i++)
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
                    file.Write(matrix[i][j] + " ");
                }

                file.Write(Environment.NewLine);
            }
        }

        private static void WriteMatrix(double[][][] matrix, ref StreamWriter file)
        {
            for (var i = 0; i < matrix.Length; i++)
            {
                file.WriteLine(i);
                for (var j = 0; j < matrix[0][0].Length; j++)
                {
                    for (var k = 0; k < matrix[0].Length; k++)
                    {
                        file.WriteLine("Vector [{0},{1}]: {2}",j+1, k, matrix[i][k][j]);
                    }
                    file.Write(Environment.NewLine);
                }
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
