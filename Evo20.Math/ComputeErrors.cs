
//namespace Evo20.Math
//{
//    public static class ComputeErrors
//    {
//        const int PCount = 13;
//        const int UCount = 100;
//        const int VCount = 5;

//        private static double[][] СomputeCodeAverage(double[][] adcCodes)
//        {
//            var averageValues=new double[PCount][];
//            for (int i = 0; i < averageValues.Length; i++)
//                averageValues[i]=new double[UCount];
//            for (int p = 1; p < PCount; p++)
//            {
//                for (int u = 0; u < UCount; u++)
//                {
//                    double sum = 0;
//                    for (int v = 0; v < VCount; v++)
//                    {
//                        sum += adcCodes[p][5 * u + v];
//                    }
//                    averageValues[p][u] = sum;
//                }
//            }
//            return averageValues;
//        }
//    }
//}
