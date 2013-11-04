using System;
using System.Collections.Generic;

namespace MQCloud.Transport.Extensions
{
    internal static class CommonExtensions {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) { // TODO Move to  common
            foreach (var element in source) {
                action(element);
            }
        }
    }
}