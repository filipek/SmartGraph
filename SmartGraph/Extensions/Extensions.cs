#region Copyright (c) 2020 Filip Fodemski
// 
// Copyright (c) 2020 Filip Fodemski
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartGraph.Common;

public static class Extensions
{
    public static string ToFlatString<K,V>(this IDictionary<K,V> dict)
    {
        Guard.AssertNotNull(dict, "dict");

        return dict.Aggregate(new StringBuilder(),
            (current, next) => current.AppendFormat(", {0}:{1}", next.Key, next.Value),
            sb => sb.Length > 2 ? sb.Remove(0, 2).ToString() : "");
    }

    public static string ToFlatString<T>(this IList<T> list)
    {
        Guard.AssertNotNull(list, "list");

        return list.Aggregate(new StringBuilder(),
            (current, next) => current.AppendFormat(", {0}", next.ToString()),
            sb => sb.Length > 2 ? sb.Remove(0, 2).ToString() : "");
    }

    public static void ForEach<T>(this IEnumerable<T> coll, Action<T> action)
    {
        Guard.AssertNotNull(coll, "coll");
        Guard.AssertNotNull(action, "action");

        foreach (var item in coll) { action(item); }
    }

    public static double NextBetween(this Random random, double min, double max)
    {
        Guard.AssertNotNull(random, "random");
        Guard.AssertSmallerThan(min, max, "min, max");

        var r = random.NextDouble();
        return r * (max - min) + min;
    }

    public static int NextBetween(this Random random, int min, int max)
    {
        Guard.AssertNotNull(random, "random");
        Guard.AssertSmallerThan(min, max, "min, max");

        var r = random.Next();
        return min + r % (max - min + 1);
    }

    public static bool None<T>(this IEnumerable<T> list)
    {
        Guard.AssertNotNull(list, "list");

        return !list.Any();
    }

    public static bool None<T>(this IEnumerable<T> list, Func<T, bool> predicate)
    {
        Guard.AssertNotNull(list, "list");
        Guard.AssertNotNull(predicate, "predicate");

        return !list.Any(predicate);
    }
}
