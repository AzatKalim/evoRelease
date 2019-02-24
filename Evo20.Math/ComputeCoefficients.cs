using System;
using System.IO;
using Evo20.Utils;

namespace Evo20.Math
{
    public static class CalculatorCoefficients
    {
        #region Constants

        private const int RawCount = 6;
        private const int KCount = 72;
        private const int LCount = 5;
        private const int SCount = 3;
        private const int MdlyCount = 4;
        private const int NdlyCount = 72;
        private const int RCount = 39;
        private const int PCount = 5;
        private const int MdysCount = 13;
        private const int NdysCount = 39;
        private const double Factor = 9.807;

        #endregion

        #region DLY coefficients

        private static double[,] ComputeTemperatureCalibrationCoefficentsDly(double[][,] adcCodes)
        {
            Log.Instance.Info("ComputeTemperatureCalibrationCoefficentsDlyД.5");

            if (adcCodes == null)
                return null;
            double[,] factors = new double[LCount, SCount];
            for (int s = 0; s < SCount; s++)
            {
                for (int k = 0; k < KCount; k++)
                {
                    factors[0, s] += adcCodes[0][s + 3, k];
                    factors[4, s] += adcCodes[4][s + 3, k];
                }
                factors[0, s] /= KCount;
                factors[4, s] /= KCount;
            }

            for (int s = 0; s < SCount; s++)
            {
                for (int k = 0; k < KCount; k++)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        factors[i, s] += adcCodes[i][s + 3, k];
                    }
                }
                for (int k = 0; k < KCount; k++)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        factors[i, s] += adcCodes[RawCount - i][s + 3, k];
                    }
                }
                for (int i = 1; i < 4; i++)
                {
                    factors[i, s] /= (KCount * 2);
                }
            }
            return factors;
        }

        //Д.3 Вычислить исходные калибровочные матрицы ДЛУ 
        private static double[][,] ComputeInitialCalibrationMatricesDly(double[][,] adcCodes)
        {
            Log.Instance.Info("Д.3");

            double[][,] initialCalibration = new double[LCount][,];
            for (int i = 0; i < initialCalibration.Length; i++)
            {
                initialCalibration[i] = new double[KCount, MdlyCount];
            }

            for (int l = 0; l < LCount; l++)
            {
                for (int n = 0; n < KCount; n++)
                {
                    initialCalibration[l][n, 0] = 1;
                    for (int m = 1; m < MdlyCount; m++)
                    {
                        if ((l == 0 || l == 4) && m > 0)
                        {
                            initialCalibration[l][n, m] = adcCodes[l][m - 1, n];
                        }
                        else
                        {
                            initialCalibration[l][n, m] = 0.5 * (adcCodes[l][m - 1, n] + adcCodes[RawCount - l][m - 1, n]);
                        }
                    }
                }

            }
            return initialCalibration;

        }

        //Д.4 вычислить вектора эталонных ускорений ДЛУ
        private static double[][] ComputeReferenceAccelerationVectorsDly()
        {
            Log.Instance.Info("Д.4");

            double[][] b = new double[SCount][];
            for (int i = 0; i < b.Length; i++)
			{
			    b[i]= new double[NdlyCount];
			}
            int n = 0;
            int firstBorder = NdlyCount / 3;
            int secondBorder = firstBorder * 2;
            for (; n < firstBorder; n++)
            {
                b[0][n] = -Factor * System.Math.Sin(System.Math.PI * n / 12);
                b[1][n] = Factor * System.Math.Cos(System.Math.PI * n / 12);
                b[2][n] = 0;
            }
            for (; n < secondBorder; n++)
            {
                b[0][n] = 0;
                b[1][n] = Factor * System.Math.Cos(System.Math.PI * (n / 12 - 2));
                b[2][n] = -Factor * System.Math.Sin(System.Math.PI * (n / 12 - 2));
            }
            for (; n < NdlyCount; n++)
            {
                b[0][n] = Factor * System.Math.Cos(System.Math.PI * (n - 54) / 12);
                b[1][n] = 0;
                b[2][n] = Factor * System.Math.Sin(System.Math.PI * (n - 54) / 12);
            }
            return b;
        }

        //Д.5 вычислить вектора калибровочных коэффициентов ДЛУ по ускорениям
        private static double[,][,] СomputeVectorOfCalibrationCoefficientsDly(double[][,] adcCodes)
        {
            Log.Instance.Info("Д.5");
            double[,][,] result= new double[LCount,SCount][,];
            double[][,] a = ComputeInitialCalibrationMatricesDly(adcCodes);
            double[][] b = ComputeReferenceAccelerationVectorsDly();
            for (int l = 0; l < LCount; l++)
            {
                Log.Instance.Info("l={0}",l);
                for (int s = 0; s < SCount; s++)
                {
                    var aL= a[l];
                    var aLTrans=aL.Transpose();
                    var bS = b[s];
                    var t11 = Matrix.Multiply(aLTrans, aL);
                    var t1 = t11.Inverse();
                    if(t11==null)
                    {
                        Log.Instance.Error("Невозможно найти обратную матрицу l={0} s={1}", l,s);                 
                        throw new ApplicationException("Невозможно найти обратную матрицу");
                    }
                    var t21 = bS.Transpose();
                    var t2 = Matrix.Multiply(aLTrans, t21);
                    //var t2 = t22.Inverse();
                    result[l, s] = Matrix.Multiply(t1,t2).Transpose();
                }
            }
            return result;
        }

        #endregion
         
        #region DYS coefficents
   
        //Вычислить калибровочные коэффициенты ДУC по температуре

        private static double[,] ComputeTemperatureCalibrationCoefficentsDys(double[][,] adcCodes)
        {
            Log.Instance.Info("ComputeTemperatureCalibrationCoefficentsDys");
            if (adcCodes == null)
            {
                return null;
            }
            double[,] factors = new double[PCount, SCount];
            for (int p = 0; p< PCount; p++)
			{
                for (int s = 0; s < SCount; s++)
                {
                    for (int r = 0; r < RCount; r++)
                    {

                        factors[p, s] = adcCodes[p][s + 3, r];
                    }
                    factors[p, s] /= RCount;
                }
            }        
            return factors;
        }

        //Д.8 Вычислить исходные калибровочные матрицы ДУС метода МНК для температурных точек
        private static double[][,] ComputeInitialCalibrationMatricesDys(double[][,] adcCodes)
        {
            Log.Instance.Info("Д.8");
            double[][,] initialCalibration = new double[PCount][,];
            for (int i = 0; i < initialCalibration.Length; i++)
            {
                initialCalibration[i] = new double[NdysCount, MdysCount];
            }

            for (int p= 0; p < PCount; p++)
            {
                for (int n = 0; n < NdysCount; n++)
                {
                    initialCalibration[p][n, 0] = 1;
                    for (int m = 1; m < 4; m++)
                    {
                        initialCalibration[p][n, m] = adcCodes[p][m - 1, n];
                    }
                    for (int m = 4; m < 7; m++)
                    {
                        initialCalibration[p][n, m] = System.Math.Pow(adcCodes[p][m - 4, n], 2);
                    }
                    for (int m = 7; m < 10; m++)
                    {
                        initialCalibration[p][n, m] = System.Math.Pow(adcCodes[p][m - 7, n], 3);
                    }
                    for (int m = 10; m < 12; m++)
                    {
                        initialCalibration[p][n, m] = System.Math.Pow(adcCodes[p][m - 10, n], 4);
                    }                 
                }

            }
            return initialCalibration;

        }

        //Д.9 Вычислить вектора эталонных угловых скоростей
        private static double[][] ComputeVectorOfStandardAngularVelocitiesDys()
        {
            Log.Instance.Info("Д.9");
            double [][] result= new double[SCount][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i]= new double[NdysCount];
            }
            for (int s = 0; s < SCount; s++)
            {
                for (int n = 0; n < NdysCount; n++)
                {
                    if (n == 0 || n == 13 || n == 26)
                    {
                        result[s][n] = 0;
                        continue;
                    }
                    if ((n > 0 && n < 26) && s == 0)
                    {
                        result[s][n] = 0;
                        continue;
                    }
                    if ((n > 0 && n < 13) && s == 2)
                    {
                        result[s][n] = 0;
                        continue;
                    }
                    if ((n > 26) && s == 2)
                    {
                        result[s][n] = 0;
                        continue;
                    }
                    if ((n > 13) && s == 1)
                    {
                        result[s][n] = 0;
                    }
                }
            }
            result[1][1] = result[2][20] = result[0][27] = 2;
            result[1][2] = result[2][21] = result[0][28] = 8;
            result[1][3] = result[2][22] = result[0][29] = 16;
            result[1][4] = result[2][23] = result[0][30] = 64;
            result[1][5] = result[2][24] = result[0][31] = 112;
            result[1][6] = result[2][25] = result[0][32] = 128;
            result[1][7] = result[2][14] = result[0][33] = -2;
            result[1][8] = result[2][15] = result[0][34] = -8;
            result[1][9] = result[2][16] = result[0][35] = -16;
            result[1][10] = result[2][17] = result[0][36] = -64;
            result[1][11] = result[2][18] = result[0][37] = -112;
            result[1][12] = result[2][19] = result[0][38] = -128;
            return result;
        }

        //Д.10 Для каждой температуры и каждой оси вычисляет вектора калибровочных коэффициентов ДУС по угловым скоростям :
        private static double[,][,] СomputVectorOfCalibrationCoefficientsDys(double[][,] adcCodes)
        {
            Log.Instance.Info("Д.10");
            double[,][,] result = new double[PCount, SCount][,];
            double[][,] a = ComputeInitialCalibrationMatricesDys(adcCodes);
            double[][] b = ComputeVectorOfStandardAngularVelocitiesDys();           
            for (int p = 0; p < PCount; p++)
            {
                for (int s = 0; s < SCount; s++)
                {
                    var aP= a[p];
                    var aPTrans=aP.Transpose();
                    var bS= b[s];
                    var mul1 = Matrix.Multiply(aPTrans, aP).Inverse();
                    if (mul1 == null)
                    {
                        Log.Instance.Error("Невозможно найти обратную матрицу p={0} s={1}", p, s);
                        throw new ApplicationException("Невозможно найти обратную матрицу");
                    }

                    var mul2 = Matrix.Multiply(aPTrans, bS.Transpose());
                    result[p, s] = Matrix.Multiply(mul1,mul2).Transpose();
                }
            }
            return result;
        }

        #endregion 

        public static bool CalculateCoefficients(double[][,] adcCodesDly, double[][,] adcCodesDys, StreamWriter file)
        {
            Log.Instance.Info("CalculateCoefficients");

            double[,][,] coefficentsDly = СomputeVectorOfCalibrationCoefficientsDly(adcCodesDly);
            double[,] temperatureCoefficentsDly = ComputeTemperatureCalibrationCoefficentsDly(adcCodesDly);

            double[,][,] coefficentsDys = СomputVectorOfCalibrationCoefficientsDys(adcCodesDys);
            double[,] temperatureCoefficentsDys = ComputeTemperatureCalibrationCoefficentsDys(adcCodesDys);

            file.WriteLine("коэффициенты ДЛУ по ускорениям");
            WriteMatrix(coefficentsDly,ref file);
            file.WriteLine("коэффициенты ДУС по угловым скоростям");
            WriteMatrix(coefficentsDys, ref file);

            file.WriteLine("коэффициенты ДЛУ по температуре ");
            WriteMatrix(temperatureCoefficentsDly, ref file);
            file.WriteLine("коэффициенты ДУC по температуре ");
            WriteMatrix(temperatureCoefficentsDys, ref file);
            return true;
        }

        #region Secondary functions

        private static void WriteMatrix(double [,] matrix, ref StreamWriter file)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    file.Write(matrix[i, j] + " ");
                }
                file.Write(Environment.NewLine);
            }
        }

        private static void WriteMatrix(double[,][,] matrix, ref StreamWriter file)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    file.Write("Vector [{0},{1}] ", i, j);
                    for (int k = 0; k < matrix[i,j].GetLength(0); k++)
                    {
                        file.Write(matrix[i, j][k,0] + " ");
                    }
                    file.Write(Environment.NewLine);
                }
            }
        }

        #endregion
    }
}
