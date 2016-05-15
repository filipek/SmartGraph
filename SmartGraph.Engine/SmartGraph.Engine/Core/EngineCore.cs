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
using System.Collections.Generic;
using SmartGraph.Engine.Dag;

namespace SmartGraph.Engine.Core
{
    internal class EngineCore : MarshalByRefObject, IEngine, IDisposable
    {
        private readonly object firstLoadSyncRoot = new object();
        private readonly string engineName;
        private readonly IGraph engineGraph;
        private readonly IEnginePipeline enginePipeline;
        private readonly IDictionary<String, EngineNode> engineNodeMap;
        private readonly IEngineBuilder engineBuilder;
        private int countOfFirstLoadsOutstanding;
        private bool allFirstLoadsDone;

        private bool CanExecute(IEngineTask task)
        {
            bool doProduce = true;

            lock (firstLoadSyncRoot)
            {
                doProduce = allFirstLoadsDone;

                if (!doProduce && task.DirtyNode.Node is IActiveNode)
                {
                    if (--countOfFirstLoadsOutstanding == 0)
                    {
                        doProduce = allFirstLoadsDone = true;
                    }
                }
            }

            return doProduce;
        }

        internal IDictionary<string, string> GetInputs(IVertex v)
        {
            return engineBuilder.GetInputs(v);
        }

        internal INode GetNode(IVertex v)
        {
            return engineNodeMap[v.Name].Node;
        }

        internal void MarkNodeAsDirty(EngineNode engineNode)
        {
            Execute(new DefaultEngineTask(this, engineNode));
        }

        internal EngineCore(string name, IEngineBuilder builder, IEnginePipeline pipeline)
        {
            engineName = name;
            engineBuilder = builder;
            engineGraph = builder.CreateGraph();
            enginePipeline = pipeline;
            engineNodeMap = new Dictionary<string, EngineNode>();
            countOfFirstLoadsOutstanding = 0;
            allFirstLoadsDone = false;

            EngineCounters.Create();
        }

        public void Execute(IEngineTask task)
        {
            if (CanExecute(task))
            {
                enginePipeline.EventHandler.Produce(task);
            }
        }

        public void Bind()
        {
            EngineCounters.Initialize();

            foreach (var vertex in engineGraph.Vertices)
            {
                var node = engineBuilder.CreateNode(vertex);
                var nodeName = node.Name;

                var engineNode = new EngineNode(this, node, vertex);
                engineNodeMap.Add(nodeName, engineNode);

                if (engineNode.Node is IActiveNode)
                {
                    ++countOfFirstLoadsOutstanding;

                    engineNode.Activate();
                }
            }

            enginePipeline.Bind(this);
        }

        public void Start()
        {
            EngineCounters.Reset();
            enginePipeline.Start();
        }

        public void Stop()
        {
            enginePipeline.Stop();
        }

        public void Dispose()
        {
            EngineCounters.Dispose();
        }

        public string Name
        {
            get { return engineName; }
        }

        public IGraph Graph { get { return engineGraph; } }

        public IEngineNode this[string nodeName]
        {
            get { return engineNodeMap[nodeName]; }
        }
    }
}