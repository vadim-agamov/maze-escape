using System.Collections.Generic;

namespace Modules.Extensions
{
    public static class RandomExtensions
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
        
        public static IList<T> Shuffle<T>(this IList<T> self)
        {
            var count = self.Count;

            for (var i = 0; i < count; ++i)
            {
                var j = UnityEngine.Random.Range(0, count);
                (self[i], self[j]) = (self[j], self[i]);
            }

            return self;
        }
    }
}