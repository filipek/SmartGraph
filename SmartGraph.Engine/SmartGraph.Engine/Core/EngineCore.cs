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

using SmartGraph.Engine.Core.Interfaces;
using SmartGraph.Engine.Dag.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartGraph.Engine.Core
{
    internal class EngineCore : MarshalByRefObject, IEngine, IDisposable
    {
        private readonly Object firstLoadSyncRoot = new Object();
        private int countOfFirstLoadsOutstanding;
        private bool allFirstLoadsDone;

        private readonly String engineName;
        private readonly IGraph engineGraph;
        private readonly IEnginePipeline enginePipeline;
        private readonly IDictionary<String, EngineNode> engineNodeMap;

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

        internal IEngineBuilder Builder { get; private set; }

        internal void MarkNodeAsDirty(EngineNode engineNode)
        {
            Execute(new DefaultEngineTask(this, engineNode));
        }

        internal EngineCore(String name, IEngineBuilder builder, IEnginePipeline pipeline)
        {
            engineName = name;
            Builder = builder;
            engineGraph = builder.CreateGraph();
            enginePipeline = pipeline;
            engineNodeMap = new Dictionary<String, EngineNode>();
            countOfFirstLoadsOutstanding = 0;
            allFirstLoadsDone = false;

            EngineCounters.Create();
        }

        public String Name
        {
            get { return engineName; }
        }

        public IGraph Graph { get { return engineGraph; } }

        public IEngineNode this[String nodeName]
        {
            get { return engineNodeMap[nodeName]; }
        }

        public IEnginePipeline Pipeline
        {
            get { return enginePipeline; }
        }

        public void Execute(IEngineTask task)
        {
            if (CanExecute(task))
            {
                enginePipeline.Produce(task);
            }
        }

        public void Bind()
        {
            EngineCounters.Initialize();

            foreach (var vertex in engineGraph.Vertices)
            {
                var node = Builder.CreateNode(vertex);
                var nodeName = node.Name;

                var engineNode = new EngineNode(this, node, vertex);
                engineNodeMap.Add(nodeName, engineNode);

                if (engineNode.Node is IActiveNode)
                {
                    ++countOfFirstLoadsOutstanding;

                    engineNode.Activate();
                }
            }

            enginePipeline.Nodes.ForEach(p => p.Bind(this));
        }

        public void Start()
        {
            EngineCounters.Reset();

            // Back to front.
            enginePipeline.Nodes.Reverse().ForEach(p => p.Start());
        }

        public void Stop()
        {
            // Front to back.
            enginePipeline.Nodes.ForEach(p => p.Stop());
        }

        public void Dispose()
        {
            EngineCounters.Dispose();
        }
    }
}