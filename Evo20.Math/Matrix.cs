// ReSharper disable CompareOfFloatsByEqualityOperator

using MathNet.Numerics.LinearAlgebra.Double;
using System;

namespace Evo20.Math
{
    public static class  Matrix
    {
        public static double[][] Inverse(this double[][] matrix)
        {
            var temp = SparseMatrix.OfRowArrays(matrix);
            //var temp = DenseMatrix.OfRowArrays(matrix);
             var res= temp.Inverse();
            return res.ToRowArrays();
        }

        private static double[,] ToArray(double[][] matrix)
        {
            var temp = new double[matrix.Length, matrix[0].Length];
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[0].Length; j++)
                    temp[i, j] = matrix[i][j];
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
