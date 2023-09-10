using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Modules.Extensions
{
    public static class TaskExtensions
    {
        public static async UniTask WhenAll<T>(IEnumerable<UniTask<T>> tasks, IProgress<float> progress)
        {
            var total = tasks.Count();
            var current = 0;

            foreach (var task in tasks)
            {
                task.ContinueWith(_ => { progress?.Report(++current / (float) total); });
            }

            await UniTask.WaitUntil(() => current >= total);
        }
        
        public static async UniTask WhenAll(this IEnumerable<UniTask> tasks, IProgress<float> progress)
        {
            var total = tasks.Count();
            var current = 0;

            foreach (var task in tasks)
            {
                task.ContinueWith(() => { progress?.Report(++current / (float) total); });
            }

            await UniTask.WaitUntil(() => current >= total);
        }
    }
}