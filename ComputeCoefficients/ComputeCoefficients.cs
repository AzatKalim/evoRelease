using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
// my libs
using PacketsLib;
//lib for matrix 
using CSML;

namespace ComputeCoefficients
{
    public static class CalculatorCoefficients
    {
        #region Constants

        const int RAW_COUNT = 6;
        const int K_COUNT = 72;
        const int L_COUNT = 5;
        const int S_COUNT = 3;
        const int MDLY_COUNT = 4;
        const int NDLY_COUNT = 72;

        const int R_COUNT = 39;
        const int P_COUNT = 5;
        const int MDYS_COUNT = 13;
        const int NDYS_COUNT = 39;
        const double FACTOR = 9.807;

        #endregion

        #region DLY coefficients
        // получить коды АЦП из коллекции пакетов
        private static double[][,] GetADCCodesDLY(PacketsCollection[] packetsCollection)
        {
            double[][,] adcCodes = new double[RAW_COUNT][,];
            for (int i = 0; i < adcCodes.Length; i++)
			{
			    adcCodes[i]= new double [packetsCollection.Length, packetsCollection[0].PositionCount];
			}
            for (int j = 0; j < packetsCollection.Length; j++)
            {
                for (int k = 0; k < packetsCollection[j].PositionCount; k++)
                {
                    double[] meanParam = packetsCollection[j].MeanA(k);
                    for (int i = 0; i < 3; i++)
                    {
                        adcCodes[i][j, k] = meanParam[i];
                    }

                    meanParam = packetsCollection[j].MeanUA(k);
                    for (int i = 3; i < RAW_COUNT; i++)
                    {
                        //переход за границы массива!!!
                        adcCodes[i][j, k] = meanParam[i - 3];
                    }
                }
            }
            return adcCodes;
        }

        //Вычислить калибровочные коэффициенты ДЛУ по температуре
        private static double[,] ComputeTemperatureCalibrationCoefficentsDLY(double[][,] adcCodes)
        {
            if (adcCodes == null)
                return null;
            double[,] factors = new double[L_COUNT, S_COUNT];
            for (int s = 0; s < S_COUNT; s++)
            {
                for (int k = 0; k < K_COUNT; k++)
                {
                    factors[0, s] += adcCodes[0][s + 3, k];
                    factors[4, s] += adcCodes[4][s + 3, k];
                }
                factors[0, s] /= K_COUNT;
                factors[4, s] /= K_COUNT;
            }

            for (int s = 0; s < S_COUNT; s++)
            {
                for (int k = 0; k < K_COUNT; k++)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        factors[i, s] += adcCodes[i][s + 3, k];
                    }
                }
                for (int k = 0; k < K_COUNT; k++)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        factors[i, s] += adcCodes[RAW_COUNT - i][s + 3, k];
                    }
                }
                for (int i = 1; i < 4; i++)
                {
                    factors[i, s] /= (K_COUNT * 2);
                }
            }
            return factors;
        }

        //Д.3 Вычислить исходные калибровочные матрицы ДЛУ 
        private static double[][,] ComputeInitialCalibrationMatricesDLY(double[][,] adcCodes)
        {
            double[][,] initialCalibration = new double[L_COUNT][,];
            for (int i = 0; i < initialCalibration.Length; i++)
            {
                initialCalibration[i] = new double[K_COUNT, MDLY_COUNT];
            }

            for (int l = 0; l < L_COUNT; l++)
            {
                for (int n = 0; n < K_COUNT; n++)
                {
                    initialCalibration[l][n, 0] = 1;
                    for (int m = 1; m < MDLY_COUNT; m++)
                    {
                        if ((l == 0 || l == 4) && m > 0)
                        {
                            initialCalibration[l][n, m] = adcCodes[l][m - 1, n];
                        }
                        else
                        {
                            initialCalibration[l][n, m] = 0.5 * (adcCodes[l][m - 1, n] + adcCodes[RAW_COUNT - l][m - 1, n]);
                        }
                    }
                }

            }
            return initialCalibration;

        }

        //Д.4 вычислить вектора эталонных ускорений ДЛУ
        private static double[][] ComputeReferenceAccelerationVectorsDLY()
        {
            double[][] b = new double[S_COUNT][];
            for (int i = 0; i < b.Length; i++)
			{
			    b[i]= new double[NDLY_COUNT];
			}
            int n = 0;
            int firstBorder = NDLY_COUNT / 3;
            int secondBorder = firstBorder * 2;
            for (; n < firstBorder; n++)
            {
                b[0][n] = -FACTOR * Math.Sin(Math.PI * n / 12);
                b[1][n] = FACTOR * Math.Cos(Math.PI * n / 12);
                b[2][n] = 0;
            }
            for (; n < secondBorder; n++)
            {
                b[0][n] = 0;
                b[1][n] = FACTOR * Math.Cos(Math.PI * (n / 12 - 2));
                b[2][n] = -FACTOR * Math.Sin(Math.PI * (n / 12 - 2));
            }
            for (; n < NDLY_COUNT; n++)
            {
                b[0][n] = FACTOR * Math.Cos(Math.PI * (n - 54) / 12);
                b[1][n] = 0;
                b[2][n] = FACTOR * Math.Sin(Math.PI * (n - 54) / 12);
            }
            return b;
        }

        //Д.5 вычислить вектора калибровочных коэффициентов ДЛУ по ускорениям
        private static double[,][,] СomputeVectorOfCalibrationCoefficientsDLY(double[][,] adcCodes)
        {
            double[,][,] result= new double[L_COUNT,S_COUNT][,];
            double[][,] a = ComputeInitialCalibrationMatricesDLY(adcCodes);
            double[][] b = ComputeReferenceAccelerationVectorsDLY();
            for (int l = 0; l < L_COUNT; l++)
            {
                for (int s = 0; s < S_COUNT; s++)
                {
                    var aL= a[l];
                    var aLTrans=aL.Transpose();
                    var bS = b[s];
                    result[l, s] = Matrix.Mul(Matrix.Mul(aLTrans,aL).Inverse(),(Matrix.Mul(aLTrans,bS.Transpose())).Inverse());
                }
            }
            return result;
        }

        #endregion
         
        #region DYS coefficents

        // получить коды АЦП из коллекции пакетов
        private static double[][,] GetADCCodesDYS(PacketsCollection[] packetsCollection)
        {
            double[][,] adcCodes = new double[RAW_COUNT][,];
            for (int i = 0; i < adcCodes.Length; i++)
            {
                adcCodes[i] = new double[packetsCollection.Length, packetsCollection[0].PositionCount];
            }
            for (int j = 0; j < packetsCollection.Length; j++)
            {
                for (int k = 0; k < packetsCollection[j].PositionCount; k++)
                {
                    double[] meanParam = packetsCollection[k].MeanW(k);
                    for (int i = 0; i < RAW_COUNT / 2; i++)
                    {
                        adcCodes[i][j, k] = meanParam[i];
                    }

                    meanParam = packetsCollection[k].MeanUW(k);
                    for (int i = RAW_COUNT / 2; i < RAW_COUNT; i++)
                    {
                        adcCodes[i][j, k] = meanParam[i - RAW_COUNT / 2];
                    }
                }
            }
            return adcCodes;
        }

        //Вычислить калибровочные коэффициенты ДУC по температуре
        private static double[,] ComputeTemperatureCalibrationCoefficentsDYS(double[][,] adcCodes)
        {
            if (adcCodes == null)
            {
                return null;
            }
            double[,] factors = new double[P_COUNT, S_COUNT];
            for (int p = 0; p< P_COUNT; p++)
			{
                for (int s = 0; s < S_COUNT; s++)
                {
                    for (int r = 0; r < R_COUNT; r++)
                    {

                        factors[p, s] = adcCodes[p][s + 3, r];
                    }
                    factors[p, s] /= R_COUNT;
                }
            }        
            return factors;
        }

        //Д.8 Вычислить исходные калибровочные матрицы ДУС метода МНК для температурных точек
        private static double[][,] ComputeInitialCalibrationMatricesDYS(double[][,] adcCodes)
        {
            double[][,] initialCalibration = new double[P_COUNT][,];
            for (int i = 0; i < initialCalibration.Length; i++)
            {
                initialCalibration[i] = new double[NDYS_COUNT, MDYS_COUNT];
            }

            for (int p= 0; p < P_COUNT; p++)
            {
                for (int n = 0; n < NDYS_COUNT; n++)
                {
                    initialCalibration[p][n, 0] = 1;
                    for (int m = 1; m < 4; m++)
                    {
                        initialCalibration[p][n, m] = adcCodes[p][m - 1, n];
                    }
                    for (int m = 4; m < 7; m++)
                    {
                        initialCalibration[p][n, m] = Math.Pow(adcCodes[p][m - 4, n],2);
                    }
                    for (int m = 7; m < 10; m++)
                    {
                        initialCalibration[p][n, m] = Math.Pow(adcCodes[p][m - 7, n],3);
                    }
                    for (int m = 10; m < 12; m++)
                    {
                        initialCalibration[p][n, m] = Math.Pow(adcCodes[p][m - 10, n], 4);
                    }                 
                }

            }
            return initialCalibration;

        }

        //Д.9 Вычислить вектора эталонных угловых скоростей
        private static double[][] ComputeVectorOfStandardAngularVelocitiesDYS()
        {
            double [][] result= new double[S_COUNT][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i]= new double[NDYS_COUNT];
            }
            for (int s = 0; s < S_COUNT; s++)
            {
                for (int n = 0; n < NDYS_COUNT; n++)
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
                        continue;
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
        private static double[,][,] СomputVectorOfCalibrationCoefficientsDYS(double[][,] adcCodes)
        {
            double[,][,] result = new double[P_COUNT, S_COUNT][,];
            double[][,] a = ComputeInitialCalibrationMatricesDYS(adcCodes);
            double[][] b = ComputeVectorOfStandardAngularVelocitiesDYS();           
            for (int p = 0; p < P_COUNT; p++)
            {
                for (int s = 0; s < S_COUNT; s++)
                {
                    var aP= a[p];
                    var aPTrans=aP.Transpose();
                    var bS= b[s];
                    result[p, s] = Matrix.Mul(Matrix.Mul(aPTrans,aP).Inverse(),Matrix.Mul(aPTrans, bS.Transpose())).Inverse();
                }
            }
            return result;
        }

        #endregion 


        public static bool CalculateCoefficients(PacketsCollection[] packetsDLY, PacketsCollection[] packetsDYS,StreamWriter file)
        {
            double[][,] adcCodesDLY= GetADCCodesDLY(packetsDLY);
            double[,][,] coefficentsDLY = СomputeVectorOfCalibrationCoefficientsDLY(adcCodesDLY);
            double[,] temperatureCoefficentsDLY = ComputeTemperatureCalibrationCoefficentsDLY(adcCodesDLY);

            double[][,] adcCodesDYS = GetADCCodesDYS(packetsDYS);
            double[,][,] coefficentsDYS = СomputVectorOfCalibrationCoefficientsDYS(adcCodesDYS);
            double[,] temperatureCoefficentsDYS = ComputeTemperatureCalibrationCoefficentsDYS(adcCodesDYS);

            file.WriteLine("коэффициенты ДЛУ по ускорениям");
            WriteMatrix(coefficentsDLY,ref file);
            file.WriteLine("коэффициенты ДУС по угловым скоростям");
            WriteMatrix(coefficentsDYS, ref file);

            file.WriteLine("коэффициенты ДЛУ по температуре ");
            WriteMatrix(temperatureCoefficentsDLY, ref file);
            file.WriteLine("коэффициенты ДУC по температуре ");
            WriteMatrix(temperatureCoefficentsDYS, ref file);
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
