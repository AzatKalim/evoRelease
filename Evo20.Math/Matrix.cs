// ReSharper disable CompareOfFloatsByEqualityOperator

using MathNet.Numerics.LinearAlgebra.Double;

namespace Evo20.Math
{
    public static class  Matrix
    {
        public static double[][] Inverse1(this double[][] matrix)
        {
            double[,] a = matrix.ToArray();
            int info;
            alglib.matinvreport rep;
            alglib.rmatrixinverse(ref a, out info, out rep);
            //var temp = DenseMatrix.OfArray(matrix.ToArray());
            //var temp = DenseMatrix.OfRowArrays(matrix);
             //var res= temp.Inverse();
            return a.ToRowsArray();
        }

        public static double[][] Inverse(this double[][] matrix)
        {
            var temp = DenseMatrix.OfArray(matrix.ToArray());
            //var temp = DenseMatrix.OfRowArrays(matrix);
            var res= temp.Inverse();
            return res.ToColumnArrays();
        }

        public static double[,] ToArray(this double[][] matrix)
        { 
            var temp = new double[matrix.Length, matrix[0].Length];
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[0].Length; j++)
                    temp[i, j] = matrix[i][j];
            return temp;
        }

        public static double[][] ToRowsArray(this double[,] matrix)
        {
            var temp = new double[matrix.GetLength(0)][];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i]=new double[matrix.GetLength(1)];
                for (int j = 0; j < temp[i].Length; j++)
                    temp[i][j] = matrix[i,j];
            }

            return temp;
        }

        public static double[][] Multiply(double[][] A, double[][] B)
        {
            var m1 = DenseMatrix.OfRowArrays(A);
            var m2 = DenseMatrix.OfRowArrays(B);
            return m1.Multiply(m2).ToRowArrays();
        }


        public static double[][] Transpose(this double[][] matrix)
        {
            int w = matrix.Length;
            int h = matrix[0].Length;

            var result = new double[h][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j][i] = matrix[i][j];
                }
            }
            return result;
        }

        public static double[,] Transpose( this double[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            double[,] result = new double[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }

        public static double[,] Transpose(this double[] matrix)
        {
            int w = matrix.GetLength(0);
            var result = new double[w, 1];
            for (int i = 0; i < w; i++)
            {         
                result[i, 0] = matrix[i];
            }
            return result;
        }    
    }
}
