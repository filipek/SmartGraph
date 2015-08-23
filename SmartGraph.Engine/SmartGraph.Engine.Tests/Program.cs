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

using SmartGraph.Engine.Common;
using System;

namespace SmartGraph.Engine.Tests
{
    class Program
    {
        private static TestEngineHost CreateEngine(String engineName)
        {
            TestEngineHost engine;

            switch (engineName)
            {
                case "BondPricer":
                    engine = new TestEngineHost("BondPricer", "Bond_Pricer");
                    break;
                case "TickerTester":
                    engine = new TestEngineHost("TickerTester", "Value");
                    break;
                case "TaskSimulator":
                    engine = new TestEngineHost("TaskSimulator", "SimulatedTask");
                    break;
                case "RandomTester":
                default:
                    engine = new TestEngineHost("RandomTester", "publisher");
                    break;
            }

            return engine;
        }

        private static void Main(String[] args)
        {
            try
            {
                String engineName;
                if (args == null || args.Length == 0 || String.IsNullOrEmpty(args[0]))
                {
                    engineName = String.Empty;
                }
                else
                {
                    engineName = args[0];
                }

                using (var engine = CreateEngine(engineName))
                {
                    engine.Start();

                    int ch = 0;
                    while (ch == 0) { ch = Console.Read(); }

                    engine.Stop();
                }
            }
            catch (Exception e)
            {
                Diagnostics.DebugException(e, "Program failure");
            }
        }
    }
}
