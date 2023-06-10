using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class RandomUtils
    {
        public static T GetRandomItem<T>(this IReadOnlyCollection<T> self, System.Random random = null)
        {
            if (self.Count == 0)
                return default;

            random ??= new System.Random();
            var index = random.Next(0, self.Count);
            
            return self switch
            {
                IList<T> list => list[index],
                IReadOnlyList<T> readOnlyList => readOnlyList[index],
                _ => self.Skip(index).First()
            };
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