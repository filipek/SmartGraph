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
using System.Xml;
using SmartGraph.Engine.Common;
using SmartGraph.Engine.Core;
using SmartGraph.Engine.Xml;
using SmartGraph.Engine.Xml.Schema;

namespace SmartGraph.Engine.TestApp
{
    internal class TestEngineHost : IDisposable
    {
        private readonly SmartEngine engine;
        private readonly String name;
        private readonly String publishEventPattern;

        private void OnCleanNodeHandler(IEngine engine, INode node)
        {
            var nodeName = node.Name;

            if (!String.IsNullOrEmpty(publishEventPattern))
            {
                if (nodeName.Contains(publishEventPattern))
                {
                    Console.WriteLine("{0} published new value: {1}", nodeName, engine[nodeName].Value);
                }
            }
            else
            {
                Console.WriteLine("{0} published new value: {1}", nodeName, engine[nodeName].Value);
            }
        }

        public TestEngineHost(String name) : this(name, null) { }

        public TestEngineHost(String name, String containsPattern)
        {
            Guard.AssertNotNullOrEmpty(name, "name");

            this.name = name;
            publishEventPattern = containsPattern;

            var file = Helpers.DataDir(String.Format("{0}.xml", name));
            CEngine engineXml;
            using (var xmlReader = new XmlTextReader(file))
            {
                engineXml = (CEngine)XmlHelpers.Deserialize(
                    typeof(CEngine), xmlReader, XmlEngineBuilder.EngineNamespace);
            }

            var builder = new XmlEngineBuilder(engineXml);
            engine = new SmartEngine(name, builder);
            engine.Bind();

            engine.Publisher.CleanNodeEvent += (e, n) => OnCleanNodeHandler(e, n);
        }

        public void Start()
        {
            engine.Start();
        }

        public void Stop()
        {
            engine.Stop();
        }

        public void Dispose()
        {
            engine.Dispose();
        }
    }
}
