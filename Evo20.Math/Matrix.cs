// ReSharper disable CompareOfFloatsByEqualityOperator
namespace Evo20.Math
{
    public static class  Matrix
    {
        // Return the matrix's inverse or null if it has none.
        public static double[,] Inverse(this double[,] matrix)
        {
            const double tiny = 0.00001;

            // Build the augmented matrix.
            int numRows = matrix.GetUpperBound(0) + 1;
            double[,] augmented = new double[numRows, 2 * numRows];
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numRows; col++)
                    augmented[row, col] = matrix[row, col];
                augmented[row, row + numRows] = 1;
            }

            // num_cols is the number of the augmented matrix.
            int numCols = 2 * numRows;

            // Solve.
            for (int row = 0; row < numRows; row++)
            {
                // Zero out all entries in column r after this row.
                // See if this row has a non-zero entry in column r.
                if (System.Math.Abs(augmented[row, row]) < tiny)
                {
                    // Too close to zero. Try to swap with a later row.
                    for (int r2 = row + 1; r2 < numRows; r2++)
                    {
                        if (System.Math.Abs(augmented[r2, row]) > tiny)
                        {
                            // This row will work. Swap them.
                            for (int c = 0; c < numCols; c++)
                            {
                                double tmp = augmented[row, c];
                                augmented[row, c] = augmented[r2, c];
                                augmented[r2, c] = tmp;
                            }
                            break;
                        }
                    }
                }

                // If this row has a non-zero entry in column r, use it.
                if (System.Math.Abs(augmented[row, row]) > tiny)
                {
                    // Divide the row by augmented[row, row] to make this entry 1.
                    for (int col = 0; col < numCols; col++)
                        if (col != row)
                            augmented[row, col] /= augmented[row, row];
                    augmented[row, row] = 1;

                    // Subtract this row from the other rows.
                    for (int row2 = 0; row2 < numRows; row2++)
                    {
                        if (row2 != row)
                        {
                            double factor = augmented[row2, row] / augmented[row, row];
                            for (int col = 0; col < numCols; col++)
                                augmented[row2, col] -= factor * augmented[row, col];
                        }
                    }
                }
            }

            // See if we have a solution.
            //          if (augmented[numRows - 1, numRows - 1] == 0) return null;

            // Extract the inverse array.
            double[,] inverse = new double[numRows, numRows];
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numRows; col++)
                {
                    inverse[row, col] = augmented[row, col + numRows];
                }
            }

            return inverse;
        }
        // Multiply two matrices.
        //public static double[,] Mul(double[,] m1, double[,] m2)
        //{
        //    int numRows = m1.GetUpperBound(0) + 1;
        //    double[,] result = new double[numRows, numRows];
        //    for (int row = 0; row < numRows; row++)
        //    {
        //        for (int col = 0; col < numRows; col++)
        //        {
        //            double value = 0;
        //            for (int i = 0; i < numRows; i++)
        //                value += m1[row, i] * m2[i, col];
        //            result[row, col] = value;
        //        }
        //    }

        //    return result;
        //}
        public static double[,] Multiply(double[,] m1, double[,] m2)
        {
            if (m1.GetLength(1) == m2.GetLength(0))
            {
                var multiply = new double[m1.GetLength(0), m2.GetLength(1)];
                for (int i = 0; i < multiply.GetLength(0); i++)
                {
                    for (int j = 0; j < multiply.GetLength(1); j++)
                    {
                        multiply[i, j] = 0;
                        for (int k = 0; k < m1.GetLength(1); k++) // OR k<b.GetLength(0)
                            multiply[i, j] = multiply[i, j] + m1[i, k] * m2[k, j];
                    }
                }

                return multiply;
            }

            return null;
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
            double[,] result = new double[w, 1];
            for (int i = 0; i < w; i++)
            {         
                result[i, 0] = matrix[i];
            }
            return result;
        }
       
    }
}
