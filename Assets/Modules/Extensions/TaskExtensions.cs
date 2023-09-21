using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Modules.Extensions
{
    public static class TaskExtensions
    {
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static async UniTask WhenAll(this IEnumerable<UniTask> tasks, IProgress<float> progress)
        {
            var total = tasks.Count();
            var current = 0;
            var exceptions = new List<Exception>();

            foreach (var task in tasks)
            {
                task.ContinueWith(
                        () => progress.Report(++current / (float)total),
                        exception => { exceptions.Add(exception); })
                    .Forget();
            }
            
            await UniTask.WaitUntil(() => current >= total || exceptions.Count > 0);
            
            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        private static async UniTask ContinueWith(this UniTask task, Action continuationFunction, Action<Exception> exceptionHandler)
        {
            try
            {
                await task;
                continuationFunction.Invoke();
            }
            catch (Exception e)
            {
                exceptionHandler.Invoke(e);
            }
        }
    }
}