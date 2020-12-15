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

using System;

namespace SmartGraph.RandomGraphGenerator.Generators
{
    /// <summary>
    /// Code ported from: http://stackoverflow.com/questions/12790337/generating-a-random-dag.
    /// Outputs in the dot format which can easily be displayed in gvedit (part of GraphViz).
    /// </summary>
    internal class RandomGraphGenerator
    {
        public static class Default
        {
            public const int MinRank = 3;
            public const int MaxRank = 5;
            public const int MinPerRank = 1;
            public const int MaxPerRank = 5;
            public const int Percent = 30;
        }

        public RandomGraphGenerator()
        {
            MinRank = Default.MinRank;
            MaxRank = Default.MaxRank;
            MinPerRank = Default.MinPerRank;
            MaxPerRank = Default.MaxPerRank;
            Percent = Default.Percent;
        }

        public RandomGraphGenerator(int minRank, int maxRank, int minPerRank, int maxPerRank, int percent)
        {
            MinRank = minRank;
            MaxRank = maxRank;
            MinPerRank = minPerRank;
            MaxPerRank = maxPerRank;
            Percent = percent;
        }

        public Action OnStartGraph { get; set; }
        public Action OnEndGraph { get; set; }
        public Action<int> OnCreateVertex { get; set; }
        public Action<int, int> OnCreateEdge { get; set; }

        public int MinRank { get; private set; }
        public int MaxRank { get; private set; }
        public int MinPerRank { get; private set; }
        public int MaxPerRank { get; private set; }
        public int Percent { get; private set; }

        public void Generate()
        {
            Generate(OnStartGraph, OnEndGraph, OnCreateEdge, OnCreateVertex);
        }

        public void Generate(
            Action onStartGraph,
            Action onEndGraph,
            Action<int, int> onCreateEdge,
            Action<int> onCreateVertex)
        {
            var random = new Random();
            int ranks = random.NextBetween(MinRank, MaxRank);
            int nodeCount = 0;

            if (onStartGraph != null)
            {
                onStartGraph();
            }

            for (int i = 0; i < ranks; ++i)
            {
                int newNodeCount = random.NextBetween(MinPerRank, MaxPerRank);

                for (int j = 0; j < nodeCount; ++j)
                {
                    if (onCreateVertex != null)
                    {
                        onCreateVertex(j);
                    }

                    for (int k = 0; k < newNodeCount; ++k)
                    {
                        var n = k + nodeCount;

                        if (onCreateVertex != null)
                        {
                            onCreateVertex(n);
                        }

                        if (random.Next() % 100 < Percent)
                        {
                            if (onCreateEdge != null)
                            {
                                onCreateEdge(j, n);
                            }
                        }
                    }
                }

                nodeCount += newNodeCount;
            }

            if (onEndGraph != null)
            {
                onEndGraph();
            }
        }
    }
}
