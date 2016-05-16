#region Copyright (C) 2015 Filip Fodemski
// 
// Copyright (c) 2015 Filip Fodemski
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
// (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE
//
#endregion

using System.Collections.Generic;
using SmartGraph.Engine.Common;

namespace System.Linq
{
    public static class Extensions
    {
        public static bool None<T>(this IEnumerable<T> list)
        {
            Guard.AssertNotNull(list, nameof(list));

            return !list.Any();
        }

        public static bool None<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            Guard.AssertNotNull(list, nameof(list));
            Guard.AssertNotNull(predicate, nameof(predicate));

            return !list.Any(predicate);
        }

        public static void ForEach<T>(this IEnumerable<T> coll, Action<T> action)
        {
            Guard.AssertNotNull(coll, nameof(coll));
            Guard.AssertNotNull(action, nameof(action));

            foreach (var item in coll) { action(item); }
        }
    }
}

namespace System
{
    public static class Extensions
    {
        public static void TryDispose(this Object obj)
        {
            if (obj == null) { return; }

            if (obj is IDisposable)
            {
                ((IDisposable)obj).Dispose();
            }
        }

        public static double NextBetween(this Random random, double min, double max)
        {
            Guard.AssertNotNull(random, nameof(random));
            Guard.AssertSmallerThan(min, max, String.Format("{0}, {1}", nameof(min), nameof(max)));

            var r = random.NextDouble();
            return r * (max - min) + min;
        }

        public static int NextBetween(this Random random, int min, int max)
        {
            Guard.AssertNotNull(random, nameof(random));
            Guard.AssertSmallerThan(min, max, String.Format("{0}, {1}", nameof(min), nameof(max)));

            var r = random.Next();
            return min + r % (max - min + 1);
        }
    }
}
