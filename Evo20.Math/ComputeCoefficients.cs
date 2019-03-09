using System;
using System.IO;
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

        #region matrix
        private static readonly int[][] DLY_MATRIX = {
            new [] {0, 0},
            new [] {0, 15},
            new [] {0, 30},
            new [] {0, 45},
            new [] {0, 60},
            new [] {0, 75},
            new [] {0, 90},
            new [] {0, 105},
            new [] {0, 120},
            new [] {0, 135},
            new [] {0, 150},
            new [] {0, 165},
            new [] {0, 180},
            new [] {0, 195},
            new [] {0, 210},
            new [] {0, 225},
            new [] {0, 240},
            new [] {0, 255},
            new [] {0, 270},
            new [] {0, 285},
            new [] {0, 300},
            new [] {0, 315},
            new [] {0, 330},
            new [] {0, 345},
            new [] {90, 0},
            new [] {90, 15},
            new [] {90, 30},
            new [] {90, 45},
            new [] {90, 60},
            new [] {90, 75},
            new [] {90, 90},
            new [] {90, 105},
            new [] {90, 120},
            new [] {90, 135},
            new [] {90, 150},
            new [] {90, 165},
            new [] {90, 180},
            new [] {90, 195},
            new [] {90, 210},
            new [] {90, 225},
            new [] {90, 240},
            new [] {90, 255},
            new [] {90, 270},
            new [] {90, 285},
            new [] {90, 300},
            new [] {90, 315},
            new [] {90, 330},
            new [] {90, 345},
            new [] {-90, -90},
            new [] {-75, -90},
            new [] {-60, -90},
            new [] {-45, -90},
            new [] {-30, -90},
            new [] {-15, -90},
            new [] {0, -90},
            new [] {15, -90},
            new [] {30, -90},
            new [] {45, -90},
            new [] {60, -90},
            new [] {75, -90},
            new [] {90, -90},
            new [] {105, -90},
            new [] {120, -90},
            new [] {135, -90},
            new [] {150, -90},
            new [] {165, -90},
            new [] {180, -90},
            new [] {195, -90},
            new [] {210, -90},
            new [] {225, -90},
            new [] {240, -90},
            new [] {255, -90}
        };
        #endregion

        #region DYS 

        private static readonly double[][] DYS_MATRIX =
        {
            new double [] {0, 0, 0},
            new double [] {0, 2, 0},
            new double [] {0, 8, 0},
            new double [] {0, 16, 0},
            new double [] {0, 64, 0},
            new double [] {0, 112, 0},
            new double [] {0, 128, 0},
            new double [] {0, -2, 0},
            new double [] {0, -8, 0},
            new double [] {0, -16, 0},
            new double [] {0, -64, 0},
            new double [] {0, -112, 0},
            new double [] {0, -128, 0},
            new double [] {0, 0, 0},
            new double [] {0, 0, -2},
            new double [] {0, 0, -8},
            new double [] {0, 0, -16},
            new double [] {0, 0, -64},
            new double [] {0, 0, -112},
            new double [] {0, 0, -128},
            new double [] {0, 0, 2},
            new double [] {0, 0, 8},
            new double [] {0, 0, 16},
            new double [] {0, 0, 64},
            new double [] {0, 0, 112},
            new double [] {0, 0, 128},
            new double [] {0, 0, 0},
            new double [] {2, 0, 0},
            new double [] {8, 0, 0},
            new double [] {16, 0, 0},
            new double [] {64, 0, 0},
            new double [] {112, 0, 0},
            new double [] {128, 0, 0},
            new double [] {-2, 0, 0},
            new double [] {-8, 0, 0},
            new double [] {-16, 0, 0},
            new double [] {-64, 0, 0},
            new double [] {-112, 0, 0,},
            new double [] {-128, 0, 0,}
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
                    var meanA=dly.CalibrationPacketsCollection[0].MeanA(j);
                    a[i][j][0] = 1;
                    a[i][j][1] = meanA[0];
                    a[i][j][2] = meanA[1];
                    a[i][j][3] = meanA[2];
                }
            }      
            var b = DLYModelVectors();
            return ComputeCalibrationCoefficents(a, b);
        }

        //Эталонных ускорения ДЛУ
        public static double[][] DLYModelVectors()
        {
            Log.Instance.Info("Эталонные вектора ДЛУ");

            double[][] b = new double[NdlyCount][];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = new double[SCount];
            }

            int n = 0;
            for (; n < NdlyCount; n++)
            {
                b[n][0] = -Factor * System.Math.Sin(DLY_MATRIX[n][1] * System.Math.PI / 180) *
                          System.Math.Cos(DLY_MATRIX[n][0] * System.Math.PI / 180);
                b[n][1] = Factor * System.Math.Cos(DLY_MATRIX[n][1] * System.Math.PI / 180);
                b[n][2] = -Factor * System.Math.Sin(DLY_MATRIX[n][1] * System.Math.PI / 180) *
                          System.Math.Sin(DLY_MATRIX[n][0] * System.Math.PI / 180);
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
                var aTrans = a[i].Transpose();
                var step1 = Matrix.Multiply(aTrans, a[i]);
                var step2 = step1.Inverse();
                var step3 = Matrix.Multiply(aTrans, b);
                result[i] = Matrix.Multiply(step2, step3);
            }

            return result;
        }

        private static double[][][] ComputeAMatrix(ISensor dys)
        {
            Log.Instance.Info("Вычисление матрицы А ДУС");

            var a = new double[8][][];
            for (var i = 0; i < a.Length; i++)
                a[i]= new double[RCount][];

            for (var i = 0; i < a.Length; i++)
            {
                for (var j = 0; j < RCount;j++)
                {
                    a[i][j]= new double[13];
                    var w =dys.CalibrationPacketsCollection[i].MeanW(j);
                    a[i][j][0] = 1;
                    var counter = 1;
                    for (var k = 1; k < 5; k++)
                    {
                        a[i][j][counter] = System.Math.Pow(w[0],k);
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
                for (var j = 0; j < matrix[i].Length; j++)
                {
                    for (var k = 0; k < matrix[i][j].Length; k++)
                    {
                        file.WriteLine("Vector [{0},{1}]: {2}", i, j, matrix[i][j][k]);
                    }
                    file.Write(Environment.NewLine);
                }
            }
        }

        #endregion
    }
}
