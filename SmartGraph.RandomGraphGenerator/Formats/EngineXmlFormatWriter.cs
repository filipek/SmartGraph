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

using SmartGraph.Dag.Interfaces;
using SmartGraph.Xml;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SmartGraph.RandomGraphGenerator.Formats
{
    internal class EngineXmlFormatWriter : IGraphFormatWriter
    {
        static void WriteInputs(XmlWriter xmlWriter, IVertex vertex)
        {
            xmlWriter.WriteStartElement("e", "inputs", XmlEngineBuilder.EngineNamespace);

            int inputId = 0;
            foreach (var ie in vertex.InEdges)
            {
                xmlWriter.WriteStartElement("e", "input", XmlEngineBuilder.EngineNamespace);
                xmlWriter.WriteAttributeString("name", String.Format("input-{0}", ++inputId));
                xmlWriter.WriteAttributeString("ref", String.Format("node-{0}", ie.Source.Name));
                xmlWriter.WriteEndElement(); // input
            }
            xmlWriter.WriteEndElement(); // inputs
        }

        public void Write(IGraph graph, TextWriter writer)
        {
            var random = new Random();
            var ens = XmlEngineBuilder.EngineNamespace;
            var nns = XmlEngineBuilder.NodesNamespace;

            var xmlSettings = new XmlWriterSettings();
            xmlSettings.Encoding = new UTF8Encoding(false);
            xmlSettings.Indent = true;

            var memoryStream = new MemoryStream();
            using (var xmlWriter = XmlWriter.Create(memoryStream, xmlSettings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("e", "engine", ens);
                xmlWriter.WriteAttributeString("name", "RandomTester");
                xmlWriter.WriteAttributeString("xmlns", "");
                xmlWriter.WriteAttributeString("xmlns", "e", null, ens);
                xmlWriter.WriteAttributeString("xmlns", "n", null, nns);

                var sortedVertices = graph.Vertices.ToList();
                sortedVertices.Sort(new VertexNameComparer());
                foreach (var v in sortedVertices)
                {
                    xmlWriter.WriteStartElement("e", "node", ens);

                    if (v.InEdges.Any() && v.OutEdges.Any()) // calculated node
                    {
                        xmlWriter.WriteAttributeString("name", String.Format("node-{0}", v.Name));

                        WriteInputs(xmlWriter, v);

                        xmlWriter.WriteStartElement("e", "obj", ens);
                        xmlWriter.WriteAttributeString(
                            "type",
                            "SmartGraph.Engine.Nodes.Core.SumOfInputs, SmartGraph.Engine.Nodes");

                        xmlWriter.WriteStartElement("n", "SumOfInputs", nns);
                        xmlWriter.WriteEndElement(); // SumOfInputs

                        xmlWriter.WriteEndElement(); // obj
                    }
                    else if (v.OutEdges.None()) // publisher
                    {
                        xmlWriter.WriteAttributeString("name", String.Format("node-{0}-publisher", v.Name));

                        WriteInputs(xmlWriter, v);

                        xmlWriter.WriteStartElement("e", "obj", ens);
                        xmlWriter.WriteAttributeString(
                            "type",
                            "SmartGraph.Engine.Nodes.Core.SleepJob, SmartGraph.Engine.Nodes");

                        xmlWriter.WriteStartElement("n", "SleepJob", nns);
                        xmlWriter.WriteEndElement(); // SleepJob

                        xmlWriter.WriteEndElement(); // obj
                    }
                    else if (v.InEdges.None()) // active node
                    {
                        xmlWriter.WriteAttributeString("name", String.Format("node-{0}", v.Name));

                        xmlWriter.WriteStartElement("e", "obj", ens);
                        xmlWriter.WriteAttributeString(
                            "type",
                            "SmartGraph.Engine.Nodes.Core.Ticker, SmartGraph.Engine.Nodes");

                        xmlWriter.WriteStartElement("n", "Ticker", nns);

                        xmlWriter.WriteAttributeString("random", "true");
                        xmlWriter.WriteAttributeString("minValue", "100");
                        xmlWriter.WriteAttributeString("maxValue", "1000");

                        xmlWriter.WriteEndElement(); // Ticker
                        xmlWriter.WriteEndElement(); // obj
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            String.Format("Vertex '{0}' is of unknown type.", v.Name));
                    }

                    xmlWriter.WriteEndElement(); // node
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }

            writer.Write(Encoding.UTF8.GetString(memoryStream.ToArray()));
            writer.Flush();
        }
    }
}
