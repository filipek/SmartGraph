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

using QLNet;
using SmartGraph.Engine.Common;
using System;
using System.Xml.Serialization;

namespace SmartGraph.Engine.Nodes.QLNet
{
    [Serializable]
    [XmlRoot("SimpleBondPricer", Namespace = "urn:smartgraph:engine:nodes", IsNullable = false)]
    public class SimpleBondPricer : NodeBase
    {
        public SimpleBondPricer() { }

        public override void Update()
        {
            var bond = GetInputNode<FixedRateBond>("Bond");
            var discountCurve = GetInputNode<YieldTermStructure>("DiscountCurve");

            var termStructure = new Handle<YieldTermStructure>(discountCurve);
            var bondEngine = new DiscountingBondEngine(termStructure);

            bond.setPricingEngine(bondEngine);

            Value = NetPV = bond.NPV();
            CleanPrice = bond.cleanPrice();
            DirtyPrice = bond.dirtyPrice();

            Diagnostics.WriteLine(
                this, String.Format("[{0}] NetPV = {1}", Name, NetPV));
        }

        [XmlIgnore]
        public double NetPV { get; private set; }
        
        [XmlIgnore]
        public double CleanPrice { get; private set; }

        [XmlIgnore]
        public double DirtyPrice { get; private set; }
    }
}
