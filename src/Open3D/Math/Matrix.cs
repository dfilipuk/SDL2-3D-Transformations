using System;

namespace Open3D.Math
{
    public class Matrix
    {
        private const int _rowsDemensionNumber = 0;
        private const int _columnsDemensionNumber = 1;
        private readonly int[,] _values;

        public int RowsCount => _values.GetLength(_rowsDemensionNumber);
        public int ColumnsCount => _values.GetLength(_columnsDemensionNumber);

        public int this[int i, int j]
        {
            get => _values[i, j];
            set => _values[i, j] = value;
        }

        public Matrix(int rows, int columns)
        {
            _values = new int[rows, columns];
        }

        public Matrix MultiplyBy(Matrix matrix)
        {
            if (ColumnsCount != matrix.RowsCount)
            {
                throw new ArgumentException("Wrong matrix size");
            }

            var result = new Matrix(RowsCount, matrix.ColumnsCount);

            for (int i = 0; i < result.RowsCount; i++)
            {
                for (int j = 0; j < result.ColumnsCount; j++)
                {
                    int item = 0;

                    for (int k = 0; k < ColumnsCount; k++)
                    {
                        item += this[i, k] * matrix[k, j];
                    }

                    result[i, j] = item;
                }
            }

            return result;
        }
    }
}
