using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputeCoefficients
{
    public static class ComputeErrors
    {
        const int P_COUNT = 13;
        const int U_COUNT = 100;
        const int V_COUNT = 5;

        private static double[][] СomputeCodeAverage(double[][] adcCodes)
        {
            var averageValues=new double[P_COUNT][];
            for (int i = 0; i < averageValues.Length; i++)
                averageValues[i]=new double[U_COUNT];
            for (int p = 1; p < P_COUNT; p++)
            {
                for (int u = 0; u < U_COUNT; u++)
                {
                    double sum = 0;
                    for (int v = 0; v < V_COUNT; v++)
                    {
                        sum += adcCodes[p][5 * u + v];
                    }
                    averageValues[p][u] = sum;
                }
            }
            return averageValues;
        }
    }
}
