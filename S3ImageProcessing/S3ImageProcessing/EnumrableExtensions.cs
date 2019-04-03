using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3ImageProcessing
{
    public static class EnumrableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, bool parallel, Action<T> action)
        {
            if (parallel)
            {
                var partitioner = Partitioner.Create(source, EnumerablePartitionerOptions.NoBuffering);
                Parallel.ForEach(partitioner, action);
            }
            else
            {
                foreach (var obj in source)
                {
                    action(obj);
                }
            }
        }
    }
}