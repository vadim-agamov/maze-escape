using System.Collections.Generic;

namespace Utils
{
    public static class CollectionExtensions
    {
        public static T Random<T>(this T[] list)
        {
            var index = UnityEngine.Random.Range(0, list.Length);
            return list[index];
        }
        
        public static T Random<T>(this List<T> list)
        {
            var index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
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