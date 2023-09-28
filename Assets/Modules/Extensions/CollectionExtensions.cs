using System.Collections.Generic;

namespace Modules.Extensions
{
    public static class CollectionExtensions
    {
        public static T FirstOrDefault<T>(this T[,] array, System.Predicate<T> match)
        {
            var numRows = array.GetLength(0);
            var numCols = array.GetLength(1);

            for (var row = 0; row < numRows; row++)
            {
                for (var col = 0; col < numCols; col++)
                {
                    var item = array[row, col];
                    if (match.Invoke(item))
                    {
                        return item;
                    }
                }
            }
            return default;
        }
        
        public static List<T> Shuffle<T>(this List<T> list)
        {
            for (var index = 0; index < list.Count - 1; index++)
            {
                var r = UnityEngine.Random.Range(index, list.Count);
                (list[index], list[r]) = (list[r], list[index]);
            }

            return list;
        }
        
        public static T[,] RotateArrayClockwise<T>(this T[,] src)
        {
            int width;
            int height;
            T[,] dst;

            width = src.GetUpperBound(0) + 1;
            height = src.GetUpperBound(1) + 1;
            dst = new T[height, width];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int newRow;
                    int newCol;

                    newRow = col;
                    newCol = height - (row + 1);

                    dst[newCol, newRow] = src[col, row];
                }
            }

            return dst;
        }
        
        public static T[,] RotateAntiArrayClockwise<T>(this T[,] src)
        {
            int width;
            int height;
            T[,] dst;

            width = src.GetUpperBound(0) + 1;
            height = src.GetUpperBound(1) + 1;
            dst = new T[height, width];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int newRow;
                    int newCol;

                    newRow = width - (col + 1);
                    newCol = row;

                    dst[newCol, newRow] = src[col, row];
                }
            }

            return dst;
        }
    }
}